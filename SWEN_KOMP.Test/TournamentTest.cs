using Moq;
using SWEN_KOMP.BLL.Scores;
using SWEN_KOMP.BLL.Tournaments;
using SWEN_KOMP.DAL.Tournaments;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Test
{
    [TestFixture]
    internal class TournamentTest
    {
        private Mock<ITournamentDao> _mockTournamentDao;
        private Mock<IScoreManager> _mockScoreManager;
        private TournamentManager _tournamentManager;

        [SetUp]
        public void Setup()
        {
            _mockTournamentDao = new Mock<ITournamentDao>();
            _mockScoreManager = new Mock<IScoreManager>();
            _tournamentManager = new TournamentManager(_mockTournamentDao.Object, _mockScoreManager.Object);
        }

        [Test]
        public void GetHistory_WithNoHistory_ThrowsEmptyHistoryException()
        {
            // arrange
            string username = "nonexistentuser";
            _mockTournamentDao.Setup(dao => dao.RetrieveHistory(username)).Returns(new List<HistorySchema>());

            // act und assert
            Assert.Throws<EmptyHistoryException>(() => _tournamentManager.GetHistory(username));
        }

        [Test]
        public void GetHistory_WithHistory_ReturnsHistory()
        {
            // arrange
            string username = "testuser";
            var history = new List<HistorySchema> { new HistorySchema(10, 120, username) };
            _mockTournamentDao.Setup(dao => dao.RetrieveHistory(username)).Returns(history);

            // act
            var result = _tournamentManager.GetHistory(username);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(username, result[0].Username);
        }

    }
}

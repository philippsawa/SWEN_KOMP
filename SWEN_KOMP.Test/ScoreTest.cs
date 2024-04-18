using Moq;
using SWEN_KOMP.BLL.Scores;
using SWEN_KOMP.DAL.Scores;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.Models.Schemas;

namespace SWEN_KOMP.Test
{
    [TestFixture]
    public class ScoreManagerTests
    {
        private Mock<IScoreDao> _mockScoreDao;
        private ScoreManager _scoreManager;

        [SetUp]
        public void Setup()
        {
            _mockScoreDao = new Mock<IScoreDao>();
            _scoreManager = new ScoreManager(_mockScoreDao.Object);
        }

        [Test]
        public void InsertUserStats_CallsDaoInsertUserStats()
        {
            // arrange
            string token = "authToken";

            // act
            _scoreManager.InsertUserStats(token);

            // assert
            _mockScoreDao.Verify(dao => dao.InsertUserStats(token), Times.Once);
        }

        [Test]
        public void GetScoreboard_ReturnsListOfUserStats()
        {
            // arrange
            var expectedScoreboard = new List<UserStatsSchema>
                {
                    new UserStatsSchema("John", 150, 10),
                    new UserStatsSchema("Doe", 120, 15)
                };

            _mockScoreDao.Setup(dao => dao.GetScoreboard()).Returns(expectedScoreboard);

            // act
            var result = _scoreManager.GetScoreboard();

            // assert
            Assert.AreEqual(expectedScoreboard, result);
        }

        [Test]
        public void GetSpecificUserStats_WhenUserExists_ReturnsUserStats()
        {
            // arrange
            var expectedStats = new UserStatsSchema("John", 150, 10);
            _mockScoreDao.Setup(dao => dao.GetUserStatsSchema("authToken")).Returns(expectedStats);

            // act
            var result = _scoreManager.GetSpecificUserStats("authToken");

            // assert
            Assert.AreEqual(expectedStats, result);
        }

        [Test]
        public void AddElo_CallsDaoAddEloWithCorrectAmount()
        {
            // arrange
            string authToken = "authToken";
            int amount = 10;

            // act
            _scoreManager.AddElo(amount, authToken);

            // assert
            _mockScoreDao.Verify(dao => dao.AddElo(amount, authToken), Times.Once);
        }

    }
}

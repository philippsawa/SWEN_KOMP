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
            // Arrange
            string token = "authToken";

            // Act
            _scoreManager.InsertUserStats(token);

            // Assert
            _mockScoreDao.Verify(dao => dao.InsertUserStats(token), Times.Once);
        }

        [Test]
        public void GetScoreboard_ReturnsListOfUserStats()
        {
            // Arrange
            var expectedScoreboard = new List<UserStatsSchema>
                {
                    new UserStatsSchema("John", 150, 10),
                    new UserStatsSchema("Doe", 120, 15)
                };

            _mockScoreDao.Setup(dao => dao.GetScoreboard()).Returns(expectedScoreboard);

            // Act
            var result = _scoreManager.GetScoreboard();

            // Assert
            Assert.AreEqual(expectedScoreboard, result);
        }

        [Test]
        public void GetSpecificUserStats_WhenUserExists_ReturnsUserStats()
        {
            // Arrange
            var expectedStats = new UserStatsSchema("John", 150, 10);
            _mockScoreDao.Setup(dao => dao.GetUserStatsSchema("authToken")).Returns(expectedStats);

            // Act
            var result = _scoreManager.GetSpecificUserStats("authToken");

            // Assert
            Assert.AreEqual(expectedStats, result);
        }

        [Test]
        public void AddElo_CallsDaoAddEloWithCorrectAmount()
        {
            // Arrange
            string authToken = "authToken";
            int amount = 10;

            // Act
            _scoreManager.AddElo(amount, authToken);

            // Assert
            _mockScoreDao.Verify(dao => dao.AddElo(amount, authToken), Times.Once);
        }

    }
}

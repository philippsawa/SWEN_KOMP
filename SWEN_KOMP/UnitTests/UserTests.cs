using Moq;
using NUnit.Framework;
using SWEN_KOMP.BLL.Users;
using SWEN_KOMP.DAL.Users;
using SWEN_KOMP.Models.Schemas;

namespace SWEN_KOMP.UnitTests
{
    [TestFixture]
    internal class UserTests
    {
        private Mock<IUserDao> _mockUserDao;
        private UserManager _userManager;

        [SetUp]
        public void Setup()
        {
            // IUserDao mocken
            _mockUserDao = new Mock<IUserDao>();

            // UserManager mit dem gemockten IUserDao initialisieren
            _userManager = new UserManager(_mockUserDao.Object);
        }

        [Test]
        public void UserRegistrationTest()
        {
            // Arrange
            var newUser = new UserSchema("testUser", "testPassword");

            // User und DataInsertion callen (mit dem mock) und true return wenn erfolgreich
            _mockUserDao.Setup(dao => dao.UserInsertion(It.IsAny<UserSchema>())).Returns(true);
            _mockUserDao.Setup(dao => dao.DataInsertion(It.IsAny<UserSchema>())).Returns(true);

            // Act
            _userManager.RegisterUser(newUser);

            // Assert
            _mockUserDao.Verify(dao => dao.UserInsertion(It.Is<UserSchema>(user =>
                user.Username == newUser.Username &&
                user.Password == newUser.Password &&
                user.Token == $"{newUser.Username}-sebToken")),
                Times.Once);

            _mockUserDao.Verify(dao => dao.DataInsertion(It.Is<UserSchema>(user =>
                user.Username == newUser.Username)),
                Times.Once);
        }
    }
}

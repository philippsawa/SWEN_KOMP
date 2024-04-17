using Moq;
using SWEN_KOMP.BLL.Users;
using SWEN_KOMP.DAL.Users;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.Models.Schemas;
using System.Net;

namespace SWEN_KOMP.Test
{

    public class UserTest
    {
        private Mock<IUserDao> _mockUserDao;
        private UserManager _userManager;

        [SetUp]
        public void Setup()
        {
            _mockUserDao = new Mock<IUserDao>();
            _userManager = new UserManager(_mockUserDao.Object);
        }

        [Test]
        public void RegisterWithValidCredentialsDoesNotThrow()
        {
            // arrange
            _mockUserDao.Setup(m => m.UserInsertion(It.IsAny<UserSchema>())).Returns(true);
            var newUser = new UserSchema("testusr", "testpwd");

            // act and assert
            Assert.DoesNotThrow(() => _userManager.RegisterUser(newUser));
        }

        [Test]
        public void RegisterWithExistingUserThrowsDuplicateUserException()
        {
            // arrange
            _mockUserDao.Setup(m => m.UserInsertion(It.IsAny<UserSchema>())).Returns(false);
            var existingUser = new UserSchema("testusr", "testpwd");

            // act and assert
            Assert.Throws<DuplicateUserException>(() => _userManager.RegisterUser(existingUser));
        }

        [Test]
        public void LoginWithValidCredentialsDoesNotThrowException()
        {
            // Arrange
            var userCredentials = new UserSchema("testusr", "testpwd");
            _mockUserDao.Setup(m => m.UserLogin(userCredentials)).Returns(true); // gültige credentials

            // Act & Assert
            Assert.DoesNotThrow(() => _userManager.LoginUser(userCredentials),
                "Login should succeed and not throw an exception with valid credentials.");

            // verifizieren dass userlogin methode auf den userdao mock genau EIN MAL aufgerufen wurde
            _mockUserDao.Verify(m => m.UserLogin(userCredentials), Times.Once);
        }

        [Test]
        public void LoginWithInvalidCredentialsThrowsUserNotFoundException()
        {
            // Arrange
            var userCredentials = new UserSchema("testusr", "testpwd");
            _mockUserDao.Setup(m => m.UserLogin(userCredentials)).Returns(false); // ungültige credentials

            // Act & Assert
            Assert.Throws<UserNotFoundException>(() => _userManager.LoginUser(userCredentials),
                "Login should throw UserNotFoundException with invalid credentials.");

            // verifizieren dass userlogin methode auf den userdao mock genau EIN MAL aufgerufen wurde
            _mockUserDao.Verify(m => m.UserLogin(userCredentials), Times.Once);
        }
    }
}
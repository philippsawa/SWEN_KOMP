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
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RegisterWithValidCredentialsDoesNotThrow()
        {
            // arrange
            var userDao = new Mock<IUserDao>(); // use moq < 4.20
            userDao.Setup(m => m.UserInsertion(It.IsAny<UserSchema>())).Returns(true);

            var userManager = new UserManager(userDao.Object);
            var newUser = new UserSchema("testusr", "testpwd");

            // act and assert
            Assert.DoesNotThrow(() => userManager.RegisterUser(newUser));
        }

        [Test]
        public void RegisterWithExistingUserThrowsDuplicateUserException()
        {
            // arrange
            var userDao = new Mock<IUserDao>();
            userDao.Setup(m => m.UserInsertion(It.IsAny<UserSchema>())).Returns(false);

            var userManager = new UserManager(userDao.Object);
            var existingUser = new UserSchema("testusr", "testpwd");

            // act and assert
            Assert.That(() => userManager.RegisterUser(new UserSchema(existingUser.Username, existingUser.Password)), Throws.TypeOf<DuplicateUserException>());
        }

        [Test]
        public void LoginWithValidCredentialsDoesNotThrowException()
        {
            // Arrange
            var userDao = new Mock<IUserDao>();
            var userCredentials = new UserSchema("testusr", "testpwd");
            userDao.Setup(m => m.UserLogin(userCredentials)).Returns(true); // Valid credentials

            var userManager = new UserManager(userDao.Object);

            // Act & Assert
            Assert.DoesNotThrow(() => userManager.LoginUser(userCredentials),
                "Login should succeed and not throw an exception with valid credentials.");

            // Verifizieren, dass die UserLogin Methode nur EIN MAL bei dem mocked userDao gecalled wurde
            userDao.Verify(m => m.UserLogin(userCredentials), Times.Once);
        }

        [Test]
        public void LoginWithInvalidCredentialsThrowsUserNotFoundException()
        {
            // Arrange
            var userDao = new Mock<IUserDao>();
            var userCredentials = new UserSchema("testusr", "testpwd");
            userDao.Setup(m => m.UserLogin(userCredentials)).Returns(false); // Invalid credentials

            var userManager = new UserManager(userDao.Object);

            // Act & Assert
            Assert.Throws<UserNotFoundException>(() => userManager.LoginUser(userCredentials),
                "Login should throw UserNotFoundException with invalid credentials.");

            // Verifizieren, dass die UserLogin Methode nur EIN MAL bei dem mocked userDao gecalled wurde
            userDao.Verify(m => m.UserLogin(userCredentials), Times.Once);
        }

    }
}
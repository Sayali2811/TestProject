using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementAss2.Controllers;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.DTO;
using ProductManagementAss2.Models.View;
using Xunit;

namespace ProductManagementTest
{
    public class UserAuthenticationControllerTests
    {
        private Mock<IUserAuthentication> CreateMockUserAuthentication()
        {
            return new Mock<IUserAuthentication>();
        }
        [Fact]
        public async Task Registration_ValidModelState_RedirectsToLogin()
        {
            
            var controller = new UserAuthenticationController(CreateMockUserAuthentication().Object);

            var validModel = new RegistrationModel();

            validModel.FirstName = "Test";
            validModel.LastName = "User";
            validModel.Username = "Test@gmail.com";
            validModel.Email = "Test@gmail.com";
            validModel.Password = "Test#1234";
            validModel.PasswordConfirm = "Test#1234";
            validModel.Role = "user";

            // Act
            var result = await controller.Registration(validModel) as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }
       
        [Fact]
        public async Task Registration_InvalidModel_ReturnsView()
        {
            // Arrange
            var controller = new UserAuthenticationController(CreateMockUserAuthentication().Object);
            var invalidModel = new RegistrationModel();


            // Add model errors to simulate client-side validation errors
            controller.ModelState.AddModelError("FirstName", "The FirstName field is required.");
            controller.ModelState.AddModelError("LastName", "The LastName field is required.");
            controller.ModelState.AddModelError("Email", "The Email field is required.");
            controller.ModelState.AddModelError("Password", "The Password field is required.");
            controller.ModelState.AddModelError("ConfirmPassword", "The ConfirmPassword field is required.");

            // Act
            var result = await controller.Registration(invalidModel) as ViewResult;

            
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Contains(controller.ModelState["FirstName"].Errors, error =>
                error.ErrorMessage == "The FirstName field is required.");
            Assert.Contains(controller.ModelState["LastName"].Errors, error =>
                error.ErrorMessage == "The LastName field is required.");
            Assert.Contains(controller.ModelState["Email"].Errors, error =>
                error.ErrorMessage == "The Email field is required.");
            Assert.Contains(controller.ModelState["Password"].Errors, error =>
                error.ErrorMessage == "The Password field is required.");
            Assert.Contains(controller.ModelState["ConfirmPassword"].Errors, error =>
                error.ErrorMessage == "The ConfirmPassword field is required.");
        }
      

        [Fact]
        public async Task Login_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var mockAuthService = CreateMockUserAuthentication();
            var controller = new UserAuthenticationController(mockAuthService.Object);
            var validModel = new LoginModel();

            validModel.Username = "validUsername";
            validModel.Password = "validPassword";
       

            // Act
            mockAuthService.Setup(authService => authService.LoginAsync(validModel))
            .ReturnsAsync(new Status { StatusCode = 1 });

            // Act
            var result = await controller.Login(validModel) as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("/Home/Index", result.Url);
        }
        [Fact]
        public async Task Login_InValidModel_RedirectsToLogin()
        {
            // Arrange
            var mockAuthService = CreateMockUserAuthentication();
            var controller = new UserAuthenticationController(mockAuthService.Object);
            var InvalidModel = new LoginModel();

            InvalidModel.Username = "invalidUsername";
            InvalidModel.Password = "invalidPassword";

            mockAuthService.Setup(authService => authService.LoginAsync(InvalidModel))
            .ReturnsAsync(new Status { StatusCode = 0 });

            // Act
            var result = await controller.Login(InvalidModel) as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }

    }
}


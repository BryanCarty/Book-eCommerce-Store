using NUnit.Framework;
using Book_eCommerce_Store.Models;
using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.Services.UsersService;
using Book_eCommerce_Store.Controllers;
using Moq;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Book_eCommerce_Store.Test.Controllers.Test
{
    public class UsersControllerTest
    {
        private Mock<IUsersService> _usersService;
        private UsersController usersController;

        [SetUp]
        public void Setup()
        {
            _usersService= new Mock<IUsersService>(MockBehavior.Strict);
        }

        [Test]
        public async Task Get_ReturnsSuccessfully()
        {
            var response = new Response { Data = It.IsAny<List<GetUserDTO>>(), Success = true, Message = It.IsAny<String>() };

            _usersService.Setup(x => x.Get()).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.Get();

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Get_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<String>() };

            _usersService.Setup(x => x.Get()).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.Get();

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetByID_ReturnsSuccessfully()
        {
            var response = new Response { Data = It.IsAny<GetUserDTO>(), Success = true, Message = It.IsAny<String>() };

            _usersService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.GetById(It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetByID_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<String>() };

            _usersService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.GetById(It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateUser_CreatesSuccessfully()
        {
            var response = new Response { Data = It.IsAny<GetUserDTO>(), Success = true, Message = It.IsAny<String>() };

            _usersService.Setup(x => x.CreateUser(It.IsAny<CreateUserDTO>())).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.CreateUser(It.IsAny<CreateUserDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task CreateUser_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<String>() };

            _usersService.Setup(x => x.CreateUser(It.IsAny<CreateUserDTO>())).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.CreateUser(It.IsAny<CreateUserDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateUser_UpdatesSuccessfully()
        {
            var response = new Response { Data = It.IsAny<GetUserDTO>(), Success = true, Message = It.IsAny<String>() };

            _usersService.Setup(x => x.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserDTO>())).ReturnsAsync(response);

            usersController= new UsersController(_usersService.Object);

            var result = await usersController.UpdateUserById(It.IsAny<int>(), It.IsAny<UpdateUserDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateUser_UserNotFound_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = "" };

            _usersService.Setup(x => x.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserDTO>())).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.UpdateUserById(It.IsAny<int>(), It.IsAny<UpdateUserDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateUser_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<string>() };

            _usersService.Setup(x => x.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserDTO>())).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.UpdateUserById(It.IsAny<int>(), It.IsAny<UpdateUserDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteUser_DeletedSuccessfully()
        {
            var response = new Response { Data = null, Success = true, Message = It.IsAny<string>() };

            _usersService.Setup(x => x.DeleteUser(It.IsAny<int>())).ReturnsAsync(response);

            usersController= new UsersController(_usersService.Object);

            var result = await usersController.Delete(It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteUser_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<string>() };

            _usersService.Setup(x => x.DeleteUser(It.IsAny<int>())).ReturnsAsync(response);

            usersController = new UsersController(_usersService.Object);

            var result = await usersController.Delete(It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }
    }
}
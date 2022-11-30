using NUnit.Framework;
using Book_eCommerce_Store.Models;
using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.Services.UsersService;
using Book_eCommerce_Store.Data;
using Moq;
using AutoMapper;


namespace Book_eCommerce_Store.Test.Services.Test.UserService
{
    internal class UsersServiceTest
    {
        private Mock<IMapper> _mapper;
        private Mock<DataContext> _context;
        private UsersService usersService;
        private MockDbContextAsynced<DataContext> mockContext;
        private List<User> data;

        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<IMapper>();

            data = new List<User>
            {
                new User { Name = "A", Id= 1, userType = 0 },
                new User { Name = "B", Id= 2, userType = 0 },
                new User { Name = "C", Id= 3, userType = 1 },
            };

            mockContext = new MockDbContextAsynced<DataContext>();
            mockContext.AddDbSetData(data);

            _context = new Mock<DataContext>();
            _context.Setup(c => c.Users).Returns(mockContext.Object.Users);

            _mapper.Setup(x => x.Map<GetUserDTO>(It.IsAny<User>())).Returns((User x) => new GetUserDTO { Id = x.Id, Name = x.Name, userType = x.userType });
        }

        [Test]
        public async Task Get_Async_ReturnsSuccessfully()
        {
            usersService = new UsersService(_mapper.Object, mockContext.Object);

            var response = await usersService.Get();

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as List<GetUserDTO>;

            Assert.That(responseData.Count, Is.EqualTo(3));
            Assert.That(responseData[0].Name, Is.EqualTo("A"));
        }

        [Test]
        public async Task GetById_Async_ReturnsSuccessfully()
        {
            usersService = new UsersService(_mapper.Object, mockContext.Object);

            var response = await usersService.GetById(2);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as GetUserDTO;

            Assert.That(responseData.Name, Is.EqualTo("B"));
        }


        [Test]
        public async Task UpdateUser_Async_UpdatesSuccessfully()
        {
            var updateParameters = new UpdateUserDTO { Name = "Barry" };

            usersService = new UsersService(_mapper.Object, _context.Object);

            var response = await usersService.UpdateUser(2, updateParameters);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as GetUserDTO;

            Assert.That(responseData.Name, Is.EqualTo("Barry"));
        }

        [Test]
        public async Task CreateUser_Async_CreatesSuccessfully()
        {
            var createParameters = new CreateUserDTO { Name = "Admin", userType = 1 };

            _mapper.Setup(x => x.Map<User>(It.IsAny<CreateUserDTO>())).Returns((CreateUserDTO x) => new User { Name = x.Name, userType = x.userType });

            usersService = new UsersService(_mapper.Object, _context.Object);

            var response = await usersService.CreateUser(createParameters);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as GetUserDTO;

            Assert.That(responseData.Name, Is.EqualTo("Admin"));
        }

        [Test]
        public async Task DeleteUser_Async_DeletesSuccessfully()
        {
            usersService = new UsersService(_mapper.Object, _context.Object);

            var response = await usersService.DeleteUser(3);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");
        }
    }




}

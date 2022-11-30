using NUnit.Framework;
using Book_eCommerce_Store.Models;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.Services.ProductsService;
using Book_eCommerce_Store.Data;
using Moq;
using AutoMapper;
using Book_eCommerce_Store.Data.Entities;

namespace Book_eCommerce_Store.Test.Services.Test.ProductsService
{
    internal class StationaryServiceTest
    {
        private Mock<IMapper> _mapper;
        private Mock<DataContext> _context;
        private MockDbContextAsynced<DataContext> mockContext;
        private StationaryService stationaryService;
        private List<PRODUCT> data;

        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<IMapper>();

            data = new List<PRODUCT>
            {
                new PRODUCT { Name = "A", Id= 1, Manufacturer = "X", ProductCategory = ProductCategory.Stationery },
                new PRODUCT { Name = "B", Id= 2, Manufacturer = "Y", ProductCategory = ProductCategory.Stationery },
                new PRODUCT { Name = "C", Id= 3, Manufacturer = "Z", ProductCategory = ProductCategory.Stationery },
            };

            _mapper.Setup(x => x.Map<Stationary>(It.IsAny<PRODUCT>())).Returns((PRODUCT x) => new Stationary { Id = x.Id, Name = x.Name, Manufacturer = x.Manufacturer, ProductCategory = x.ProductCategory });

            mockContext = new MockDbContextAsynced<DataContext>();
            mockContext.AddDbSetData(data);

            _context = new Mock<DataContext>();
            _context.Setup(c => c.Products).Returns(mockContext.Object.Products);
        }

        [Test]
        public async Task Get_Async_ReturnsSuccessfully()
        {
            stationaryService = new StationaryService(_mapper.Object, _context.Object);

            var response = await stationaryService.Get();

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as List<Stationary>;

            Assert.That(responseData.Count, Is.EqualTo(3));
            Assert.That(responseData[0].Name, Is.EqualTo("A"));

        }

        [Test]
        public async Task GetById_TrackStationary_Async_ReturnsSuccessfully()
        {
            stationaryService = new StationaryService(_mapper.Object, mockContext.Object);

            var response = await stationaryService.GetById(2, true);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as Stationary;

            Assert.That(responseData.Name, Is.EqualTo("B"));

        }


        [Test]
        public async Task Update_Async_UpdatesSuccessfully()
        {
            var updateParameters = new UpdateProductDTO { Manufacturer = "CASIO" };

            stationaryService = new StationaryService(_mapper.Object, _context.Object);

            var response = await stationaryService.Update(2, updateParameters);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as Stationary;

            Assert.That(responseData.Manufacturer, Is.EqualTo("CASIO"));

        }

        [Test]
        public async Task Create_Async_CreatesSuccessfully()
        {
            var createParameters = new CreateProductDTO { Name = "D", Manufacturer = "CASIO", ProductCategory = ProductCategory.Stationery };

            _mapper.Setup(x => x.Map<PRODUCT>(It.IsAny<CreateProductDTO>())).Returns((CreateProductDTO x) => new PRODUCT { Name = x.Name, Manufacturer = x.Manufacturer, ProductCategory = x.ProductCategory });

            stationaryService = new StationaryService(_mapper.Object, _context.Object);

            var response = await stationaryService.Create(createParameters);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as Stationary;

            Assert.That(responseData.Manufacturer, Is.EqualTo("CASIO"));

        }

        [Test]
        public async Task Delete_Async_DeletesSuccessfully()
        {
            stationaryService = new StationaryService(_mapper.Object, _context.Object);

            var response = await stationaryService.Delete(3);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");
        }
    }




}

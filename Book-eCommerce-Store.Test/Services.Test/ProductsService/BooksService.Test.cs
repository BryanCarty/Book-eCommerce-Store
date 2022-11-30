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
    internal class BooksServiceTest
    {
        private Mock<IMapper> _mapper;
        private Mock<DataContext> _context;
        private MockDbContextAsynced<DataContext> mockContext;
        private BooksService bookService;
        private List<PRODUCT> data;

        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<IMapper>();

            data = new List<PRODUCT>
            {
                new PRODUCT { Name = "A", Id= 1, Genre = "Action", ProductCategory = ProductCategory.Books },
                new PRODUCT { Name = "B", Id= 2, Genre = "Romance", ProductCategory = ProductCategory.Books },
                new PRODUCT { Name = "C", Id= 3, Genre = "Mystery", ProductCategory = ProductCategory.Books },
            };

            _mapper.Setup(x => x.Map<Book>(It.IsAny<PRODUCT>())).Returns((PRODUCT x) => new Book { Id = x.Id, Name = x.Name, Genre = x.Genre, ProductCategory = x.ProductCategory });

            mockContext = new MockDbContextAsynced<DataContext>();
            mockContext.AddDbSetData(data);

            _context = new Mock<DataContext>();
            _context.Setup(c => c.Products).Returns(mockContext.Object.Products);
        }

        [Test]
        public async Task Get_Async_ReturnsSuccessfully()
        {
            bookService = new BooksService(_mapper.Object, _context.Object);

            var response = await bookService.Get();

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as List<Book>;

            Assert.That(responseData.Count, Is.EqualTo(3));
            Assert.That(responseData[0].Name, Is.EqualTo("A"));

        }

        [Test]
        public async Task GetById_TrackBook_Async_ReturnsSuccessfully()
        {
            bookService = new BooksService(_mapper.Object, mockContext.Object);

            var response = await bookService.GetById(2, true);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as Book;

            Assert.That(responseData.Name, Is.EqualTo("B"));

        }


        [Test]
        public async Task Update_Async_UpdatesSuccessfully()
        {
            var updateParameters = new UpdateProductDTO { Name = "Harry Potter" };

            bookService = new BooksService(_mapper.Object, _context.Object);

            var response = await bookService.Update(2, updateParameters);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as Book;

            Assert.That(responseData.Name, Is.EqualTo("Harry Potter"));

        }

        [Test]
        public async Task Create_Async_CreatesSuccessfully()
        {
            var createParameters = new CreateProductDTO { Name = "Harry Potter", Genre = "Fantasy", ProductCategory = ProductCategory.Books };

            _mapper.Setup(x => x.Map<PRODUCT>(It.IsAny<CreateProductDTO>())).Returns((CreateProductDTO x) => new PRODUCT { Name = x.Name, Genre = x.Genre, ProductCategory = x.ProductCategory });

            bookService = new BooksService(_mapper.Object, _context.Object);

            var response = await bookService.Create(createParameters);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");

            var responseData = response.Data as Book;

            Assert.That(responseData.Name, Is.EqualTo("Harry Potter"));

        }

        [Test]
        public async Task Delete_Async_DeletesSuccessfully()
        {
            bookService = new BooksService(_mapper.Object, _context.Object);

            var response = await bookService.Delete(3);

            Assert.That(response, Is.Not.Null, "Object returned was null");
            Assert.That(response.Success, Is.True, "Was not successful");
        }
    }




}

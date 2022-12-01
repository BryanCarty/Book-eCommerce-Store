using NUnit.Framework;
using Book_eCommerce_Store.Models;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.Services.ProductsService;
using Book_eCommerce_Store.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Book_eCommerce_Store.Services.ProductsService.Factory;

namespace Book_eCommerce_Store.Test.Controllers.Test
{
    public class ProductsControllerTest
    {
        private Mock<IProductsService> _productsService;
        private Mock<IProductFactory> _productFactory;
        private ProductsController productsController;

        [SetUp]
        public void Setup()
        {
            _productFactory = new Mock<IProductFactory>(MockBehavior.Strict);
            _productsService= new Mock<IProductsService>(MockBehavior.Strict);
            _productFactory.Setup(x => x.GetProductsService(It.IsAny<ProductCategory>())).Returns(_productsService.Object);
        }

        [Test]
        public async Task Get_Book_ReturnsSuccessfully()
        {
            var response = new Response { Data = It.IsAny<List<Book>>(), Success = true, Message = It.IsAny<String>() };

            _productsService.Setup(x => x.Get()).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.Get(ProductCategory.Books);

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Get_Book_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<String>() };

            _productsService.Setup(x => x.Get()).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.Get(ProductCategory.Books);

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetByID_Book_ReturnsSuccessfully()
        {
            var response = new Response { Data = It.IsAny<Book>(), Success = true, Message = It.IsAny<String>() };

            _productsService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.GetById(ProductCategory.Books, It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetByID_Book_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<String>() };

            _productsService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.GetById(ProductCategory.Books, It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateProduct_Book_CreatesSuccessfully()
        {
            var response = new Response { Data = It.IsAny<Book>(), Success = true, Message = It.IsAny<String>() };

            _productsService.Setup(x => x.Create(It.IsAny<CreateProductDTO>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.CreateProduct(ProductCategory.Books, It.IsAny<CreateProductDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task CreateProduct_Book_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<String>() };

            _productsService.Setup(x => x.Create(It.IsAny<CreateProductDTO>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.CreateProduct(ProductCategory.Books, It.IsAny<CreateProductDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateProduct_Book_UpdatesSuccessfully()
        {
            var response = new Response { Data = It.IsAny<Book>(), Success = true, Message = It.IsAny<String>() };

            _productsService.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UpdateProductDTO>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.UpdateProductById(ProductCategory.Books, It.IsAny<int>(), It.IsAny<UpdateProductDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateProduct_Book_BookNotFound_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = "" };

            _productsService.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UpdateProductDTO>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.UpdateProductById(ProductCategory.Books, It.IsAny<int>(), It.IsAny<UpdateProductDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateProduct_Book_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<string>() };

            _productsService.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UpdateProductDTO>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.UpdateProductById(ProductCategory.Books, It.IsAny<int>(), It.IsAny<UpdateProductDTO>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteProduct_Book_DeletedSuccessfully()
        {
            var response = new Response { Data = null, Success = true, Message = It.IsAny<string>() };

            _productsService.Setup(x => x.Delete(It.IsAny<int>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.Delete(ProductCategory.Books, It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var res = result.Result as OkObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteProduct_Book_Error_Failure()
        {
            var response = new Response { Data = null, Success = false, Message = It.IsAny<string>() };

            _productsService.Setup(x => x.Delete(It.IsAny<int>())).ReturnsAsync(response);

            productsController = new ProductsController(_productFactory.Object);

            var result = await productsController.Delete(ProductCategory.Books, It.IsAny<int>());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

            var res = result.Result as ObjectResult;

            Assert.That(res.StatusCode, Is.EqualTo(500));
        }
    }
}
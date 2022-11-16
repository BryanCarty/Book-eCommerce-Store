using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.Services.ProductsService;
using Book_eCommerce_Store.Services.ProductsService.Factory;
using Microsoft.AspNetCore.Mvc;

namespace Book_eCommerce_Store.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductFactory productFactory;

        public ProductsController(IProductFactory productFactory)
        {
            this.productFactory = productFactory;
        }

        [HttpGet("/{productCategory}")]
        public async Task<ActionResult<List<GetProductDTO>>> Get(ProductCategory productCategory)
        {
            var response = await this.productFactory.GetProductsService(productCategory).Get();
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);

        }


        [HttpGet("/{productCategory}/{id}")]
        public async Task<ActionResult<GetProductDTO>> GetById(ProductCategory productCategory, int id)
        {
            var response = await this.productFactory.GetProductsService(productCategory).GetById(id);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPost("/{productCategory}")]
        public async Task<ActionResult<GetProductDTO>> CreateProduct(ProductCategory productCategory, CreateProductDTO newProduct)
        {
            var response = await this.productFactory.GetProductsService(productCategory).Create(newProduct);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPut("/{productCategory}/{id}")]
        public async Task<ActionResult<GetProductDTO>> UpdateProductById(ProductCategory productCategory, int id, UpdateProductDTO updatedProduct)
        {
            var response = await this.productFactory.GetProductsService(productCategory).Update(id, updatedProduct);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            if (response.Message == "")
            {
                response.Message = "Not Found";
                return StatusCode(StatusCodes.Status404NotFound, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpDelete("/{productCategory}/{id}")]
        public async Task<ActionResult<Response>> Delete(ProductCategory productCategory, int id)//FIx this -> Should be idempotent!
        {
            var response = await this.productFactory.GetProductsService(productCategory).Delete(id);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);

        }
    }
}
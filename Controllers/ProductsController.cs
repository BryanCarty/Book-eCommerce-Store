using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.Services.ProductsService;
using Microsoft.AspNetCore.Mvc;

namespace Book_eCommerce_Store.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService){
            this.productsService = productsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetProductDTO>>> Get()
        {
            var response = await this.productsService.Get();
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
            
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductDTO>> GetById(int id)
        {
            var response = await this.productsService.GetById(id);
            if (response.Success==true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPost]
        public async Task<ActionResult<GetProductDTO>> CreateProduct(CreateProductDTO newProduct)
        {
            var response = await this.productsService.CreateProduct(newProduct);
            if (response.Success==true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetProductDTO>> UpdateProductById(int id, UpdateProductDTO updatedProduct)
        {
            var response = await this.productsService.UpdateProduct(id, updatedProduct);
            if (response.Success==true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            if (response.Message == ""){
                response.Message = "Not Found";
                return StatusCode(StatusCodes.Status404NotFound, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<GetProductDTO>>> Delete(int id)//FIx this -> Should be idempotent!
        {
            var response = await this.productsService.DeleteProduct(id);
            if (response.Success==true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
            
        }
    }
}
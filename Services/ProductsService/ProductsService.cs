using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.data;
using Book_eCommerce_Store.DTOs.Products;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Services.ProductsService
{
    public class ProductsService : IProductsService
    {

        private readonly IMapper mapper;
        private readonly DataContext context;

        public ProductsService(IMapper mapper, DataContext context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Response<GetProductDTO>> CreateProduct(CreateProductDTO newProduct)
        {
            var response = new Response<GetProductDTO>();
            try{
                var mappedToProduct = this.mapper.Map<Product>(newProduct);
                this.context.Add(mappedToProduct);
                await this.context.SaveChangesAsync();
                response.Data = this.mapper.Map<GetProductDTO>(mappedToProduct);
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
           
            return response;
        }

        public async Task<Response<List<GetProductDTO>>> Get()
        {
            var response = new Response<List<GetProductDTO>>();
            try{
                var dbProducts = await this.context.Products.ToListAsync();
                response.Data = dbProducts.Select(p => this.mapper.Map<GetProductDTO>(p)).ToList();
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<GetProductDTO>> GetById(int id)
        {
            var response = new Response<GetProductDTO>();
            try{
                var dbProduct = await this.context.Products.FirstOrDefaultAsync(p => p.Id == id);
                response.Data = this.mapper.Map<GetProductDTO>(dbProduct);
            }catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public async Task<Response<GetProductDTO>> UpdateProduct(int id, UpdateProductDTO updatedProduct)
        {
            Response<GetProductDTO> response = new Response<GetProductDTO>();
            try{
                var productToUpdate = await this.context.Products
                    .FirstOrDefaultAsync(p => p.Id == id);
                    
                if (productToUpdate == null){
                    response.Success = false;
                    return response;
                }
                productToUpdate.Name = updatedProduct.Name;
                productToUpdate.Description = updatedProduct.Description;
                productToUpdate.PriceInCent = updatedProduct.PriceInCent;
                productToUpdate.Quantity = updatedProduct.Quantity;
                productToUpdate.ProductCategory = updatedProduct.ProductCategory;
                await this.context.SaveChangesAsync();
                response.Data = this.mapper.Map<GetProductDTO>(productToUpdate);
            }catch(Exception ex){
                response.Success=false;
                response.Message=ex.Message;
                return response;
            }

            return  response;
        }

        
        public async Task<Response<GetProductDTO>> DeleteProduct(int id)
        {
            Response<GetProductDTO> response = new Response<GetProductDTO>();
            try{
                Product product = await this.context.Products.FirstAsync(p => p.Id ==id);
                this.context.Products.Remove(product);
                await this.context.SaveChangesAsync();
            }catch(Exception ex){
                    if (ex.Message != "Sequence contains no elements."){
                        response.Success=false;
                        response.Message=ex.Message;
                        return response;
                    }
            }
            return  response;
        }

    }
}
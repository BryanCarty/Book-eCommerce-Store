using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Products;

namespace Book_eCommerce_Store.Services.ProductsService
{
    public interface IProductsService
    {
        Task<Response> Get();
        Task<Response> GetById(int id);
        Task<Response> Create(CreateProductDTO newProduct);
        Task<Response> Update(int id, UpdateProductDTO updatedProduct);
        Task<Response> Delete(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Products;

namespace Book_eCommerce_Store.Services.ProductsService
{
    public interface IProductsService
    {
        Task<Response<List<GetProductDTO>>> Get();

        Task<Response<GetProductDTO>> GetById(int id);

        Task<Response<GetProductDTO>> CreateProduct(CreateProductDTO newProduct);

        Task<Response<GetProductDTO>> UpdateProduct(int id, UpdateProductDTO updatedProduct);

        Task<Response<GetProductDTO>> DeleteProduct(int id);
    }
}
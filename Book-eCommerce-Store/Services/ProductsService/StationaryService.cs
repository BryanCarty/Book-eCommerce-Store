using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.Data.Entities;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.Models;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Services.ProductsService
{
    public class StationaryService : IProductsService
    {

        private readonly IMapper mapper;
        private readonly DataContext context;

        public StationaryService(IMapper mapper, DataContext context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Response> Get()
        {
            var response = new Response();
            try
            {
                var dbStationary = await context.Products.Where(p => p.ProductCategory == ProductCategory.Stationery).ToListAsync();
                response.Data = dbStationary.Select(p => mapper.Map<Stationary>(p)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> GetById(int id)
        {
            var response = new Response();
            try
            {
                var dbStationary = await context.Products.FirstOrDefaultAsync(p => p.ProductCategory == ProductCategory.Stationery && p.Id == id);
                response.Data = mapper.Map<Stationary>(dbStationary);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> Create(CreateProductDTO newProduct)
        {
            var response = new Response();
            try
            {
                newProduct.ProductCategory = ProductCategory.Stationery;
                newProduct.Author = null;
                newProduct.Genre = null;
                newProduct.PageCount = null;
                newProduct.Publisher = null;
                newProduct.PublicationDate = null;

                var mappedToStationary = mapper.Map<PRODUCT>(newProduct);
                context.Add(mappedToStationary);
                await context.SaveChangesAsync();
                response.Data = mapper.Map<Stationary>(mappedToStationary);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<Response> Update(int id, UpdateProductDTO updatedProduct)
        {
            var response = new Response();
            try
            {
                var StationaryToUpdate = await context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.ProductCategory == ProductCategory.Stationery);

                if (StationaryToUpdate == null)
                {
                    response.Success = false;
                    return response;
                }
                if (updatedProduct.Name != null) StationaryToUpdate.Name = updatedProduct.Name;
                if (updatedProduct.Description != null) StationaryToUpdate.Description = updatedProduct.Description;
                if (updatedProduct.PriceInCent != null) StationaryToUpdate.PriceInCent = (int)updatedProduct.PriceInCent;
                if (updatedProduct.Quantity != null) StationaryToUpdate.Quantity = (int)updatedProduct.Quantity;
                if (updatedProduct.ProductCategory != null) StationaryToUpdate.ProductCategory = (ProductCategory)updatedProduct.ProductCategory;
                if (updatedProduct.Manufacturer != null) StationaryToUpdate.Manufacturer = updatedProduct.Manufacturer;
                if (updatedProduct.Brand != null) StationaryToUpdate.Brand = updatedProduct.Brand;

                await context.SaveChangesAsync();
                response.Data = mapper.Map<Stationary>(StationaryToUpdate);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

            return response;
        }

        public async Task<Response> Delete(int id)
        {
            var response = new Response();
            try
            {
                var product = await context.Products.FirstAsync(p => p.Id == id && p.ProductCategory == ProductCategory.Stationery);
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.Message != "Sequence contains no elements.")
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    return response;
                }
            }
            return response;
        }
    }
}
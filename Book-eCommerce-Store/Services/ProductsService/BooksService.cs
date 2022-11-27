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
    public class BooksService : IProductsService
    {

        private readonly IMapper mapper;
        private readonly DataContext context;

        public BooksService(IMapper mapper, DataContext context)
        {
            this.context = context;
            this.mapper = mapper;
        }


        public async Task<Response> Get()
        {
            var response = new Response();
            try
            {
                var dbBooks = await context.Products.Where(p => p.ProductCategory == ProductCategory.Books).ToListAsync();
                response.Data = dbBooks.Select(p => mapper.Map<Book>(p)).ToList();
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
                var dbBook = await context.Products.FirstOrDefaultAsync(p => p.Id == id && p.ProductCategory == ProductCategory.Books);
                response.Data = mapper.Map<Book>(dbBook);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> Create(CreateProductDTO newBook)
        {
            var response = new Response();
            try
            {
                newBook.ProductCategory = ProductCategory.Books;
                newBook.Brand = null;
                newBook.Manufacturer = null;

                var mappedToBook = mapper.Map<PRODUCT>(newBook);
                context.Add(mappedToBook);
                await context.SaveChangesAsync();
                response.Data = mapper.Map<Book>(mappedToBook);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<Response> Update(int id, UpdateProductDTO updatedBook)
        {
            var response = new Response();
            try
            {
                var bookToUpdate = await context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.ProductCategory == ProductCategory.Books);

                if (bookToUpdate == null)
                {
                    response.Success = false;
                    return response;
                }
                if(updatedBook.Name != null) bookToUpdate.Name = updatedBook.Name;
                if (updatedBook.Description != null) bookToUpdate.Description = updatedBook.Description;
                if (updatedBook.PriceInCent != null) bookToUpdate.PriceInCent = (int)updatedBook.PriceInCent;
                if (updatedBook.Quantity != null) bookToUpdate.Quantity = (int)updatedBook.Quantity;
                if (updatedBook.ProductCategory != null) bookToUpdate.ProductCategory = (ProductCategory)updatedBook.ProductCategory;
                if (updatedBook.Author != null) bookToUpdate.Author = updatedBook.Author;
                if (updatedBook.Genre != null) bookToUpdate.Genre = updatedBook.Genre;
                if (updatedBook.PageCount != null) bookToUpdate.PageCount = updatedBook.PageCount;
                if (updatedBook.Publisher != null) bookToUpdate.Publisher = updatedBook.Publisher;
                if (updatedBook.PublicationDate != null) bookToUpdate.PublicationDate = updatedBook.PublicationDate;

                await context.SaveChangesAsync();
                response.Data = mapper.Map<Book>(bookToUpdate);
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
                var book = await context.Products.FirstAsync(p => p.Id == id);
                context.Products.Remove(book);
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
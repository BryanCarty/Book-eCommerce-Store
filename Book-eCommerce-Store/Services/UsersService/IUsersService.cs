using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.Models;

namespace Book_eCommerce_Store.Services.UsersService
{
    public interface IUsersService
    {
        Task<Response> Get();

        Task<Response> GetById(int id);

        Task<Response> CreateUser(CreateUserDTO newProduct);

        Task<Response> UpdateUser(int id, UpdateUserDTO updatedProduct);

        Task<Response> DeleteUser(int id);
    }
}
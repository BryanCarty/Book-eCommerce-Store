using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.Models;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Services.UsersService
{
    public class UsersService : IUsersService
    {

        private readonly IMapper mapper;
        private readonly DataContext context;

        public UsersService(IMapper mapper, DataContext context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Response> CreateUser(CreateUserDTO newUser)
        {
            var response = new Response();
            try{
                var mappedToUser = this.mapper.Map<User>(newUser);
                this.context.Add(mappedToUser);
                await this.context.SaveChangesAsync();
                response.Data = this.mapper.Map<GetUserDTO>(mappedToUser);
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
           
            return response;
        }

        public async Task<Response> Get()
        {
            var response = new Response();
            try{
                var dbUsers = await this.context.Users.ToListAsync();
                response.Data = dbUsers.Select(user => this.mapper.Map<GetUserDTO>(user)).ToList();
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> GetById(int id)
        {
            var response = new Response();
            try{
                var dbUser = await this.context.Users.FirstOrDefaultAsync(user => user.Id == id);
                response.Data = this.mapper.Map<GetUserDTO>(dbUser);
            }catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public async Task<Response> UpdateUser(int id, UpdateUserDTO updatedUser)
        {
            var response = new Response();
            try{
                var userToUpdate = await this.context.Users
                    .FirstOrDefaultAsync(user => user.Id == id);
                    
                if (userToUpdate == null){
                    response.Success = false;
                    return response;
                }
                if(updatedUser.Name != null) userToUpdate.Name = updatedUser.Name;
                if (updatedUser.userType != null) userToUpdate.userType = (int)updatedUser.userType;
                if (updatedUser.Email != null) userToUpdate.Email = updatedUser.Email;
                if (updatedUser.PhoneNumber != null) userToUpdate.PhoneNumber = updatedUser.PhoneNumber;

                await this.context.SaveChangesAsync();

                response.Data = this.mapper.Map<GetUserDTO>(userToUpdate);
            }catch(Exception ex){
                response.Success=false;
                response.Message=ex.Message;
                return response;
            }

            return  response;
        }

        
        public async Task<Response> DeleteUser(int id)
        {
            var response = new Response();
            try{
                User user = await this.context.Users.FirstAsync(user => user.Id ==id);
                this.context.Users.Remove(user);
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
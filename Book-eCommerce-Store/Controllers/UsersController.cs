using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.Models;
using Book_eCommerce_Store.Services.UsersService;
using Microsoft.AspNetCore.Mvc;

namespace Book_eCommerce_Store.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<ActionResult<List<GetUserDTO>>> Get()
        {
            var response = await this._userService.Get();
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDTO>> GetById(int id)
        {
            var response = await this._userService.GetById(id);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPost]
        public async Task<ActionResult<GetUserDTO>> CreateUser(CreateUserDTO newUser)
        {
            var response = await this._userService.CreateUser(newUser);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<GetUserDTO>> UpdateUserById(int id, UpdateUserDTO updatedUser)
        {
            var response = await this._userService.UpdateUser(id, updatedUser);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete(int id)
        {
            var response = await this._userService.DeleteUser(id);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);

        }
    }
}
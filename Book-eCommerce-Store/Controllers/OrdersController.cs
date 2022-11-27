using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Orders;
using Book_eCommerce_Store.Services.OrdersService;
using Microsoft.AspNetCore.Mvc;

namespace Book_eCommerce_Store.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService){
            this.ordersService = ordersService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetOrderDTO>>> Get()
        {
            var response = await this.ordersService.Get();
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetOrderDTO>> GetById(int id)
        {
            var response = await this.ordersService.GetById(id);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            else if(response.Message == "an order with this id does not exist")
            {
                return StatusCode(StatusCodes.Status404NotFound, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPost]
        public async Task<ActionResult<GetOrderDTO>> CreateOrder(CreateOrderDTO newOrder)
        {
            var response = await this.ordersService.CreateOrder(newOrder);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            else if(response.Message.Count()>36 && response.Message.Substring(0,36) == "Order can't be placed. We only have ")
            {
                return Ok(response);
            }
            if(response.Message == "products list is empty" || response.Message == "a product you listed does not exist")
            {
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<GetOrderDTO>> UpdateOrderById(int id, UpdateOrderDTO updatedOrder)
        {
            var response = await this.ordersService.UpdateOrder(id, updatedOrder);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            else if(response.Message.Count()>36 && response.Message.Substring(0,36) == "Order can't be updated. We only have")
            {
                return Ok(response);
            }else if(response.Message.Count()>20 && response.Message.Substring(0,20) == "Sorry, this order is"){
                return Ok(response);
            }
            if(response.Message == "products list is null" || response.Message == "a product you listed does not exist" || response.Message == "an order with this id does not exist")
            {
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete(int id)
        {
            var response = await this.ordersService.DeleteOrder(id);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }else if(response.Message.Count()>20 && response.Message.Substring(0,20) == "Sorry, this order is"){
                return Ok(response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);

        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<GetOrderDTO>> UpdateOrderById(int id, OrderStatus status)
        {
            var response = await this.ordersService.UpdateOrderStatus(id, status.status);
            if (response.Success == true)
            {
                response.Message = "Http Status OK";
                return Ok(response);
            }
            else if(response.Message == "an order with this id does not exist")
            {
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
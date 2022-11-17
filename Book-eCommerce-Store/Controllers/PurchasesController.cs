using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Book_eCommerce_Store.DTOs.Purchases;
using Book_eCommerce_Store.Services.PurchasesService;
namespace Book_eCommerce_Store.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchasesService purchasesService;

        public PurchasesController(IPurchasesService purchasesService){
            this.purchasesService = purchasesService;
        }


        [HttpPost]
        public async Task<ActionResult<GetPurchaseDTO>> CreatePurchase(CreatePurchaseDTO newPurchase)
        {
            var response = await this.purchasesService.CreatePurchase(newPurchase);
            if (response.Success==true)
            {
                return Ok(response);
            }else if(response.Message=="A product with this id does not exist"){
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }


        [HttpDelete("{product_id}/{purchase_id}")]
        public async Task<ActionResult<Response>> ReturnPurchase(int product_id, int purchase_id)
        {
            var response = await this.purchasesService.ReturnPurchase(product_id, purchase_id);
            if (response.Success==true)
            {
                return Ok(response);
            }else if(response.Message=="A product with this id does not exist"){
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }else if(response.Message == "We do not have this purchase on record"){
                return StatusCode(StatusCodes.Status200OK, response);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, response);
            
        }
    }
}
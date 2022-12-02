using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.Services.ObserverService;
using Microsoft.AspNetCore.Mvc;

namespace Book_eCommerce_Store.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObserverController : ControllerBase
    {
        private readonly IObserverService _observerService;
        public ObserverController(IObserverService observerService){
            _observerService = observerService;

        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<Response>> GetAll(){
           return Ok(await _observerService.GetAllObservers());
        }

        [HttpGet("observer/{observerid}")]
        public async Task<ActionResult<Response>> GetSingleByObserverId(int observerid){
            return Ok(await _observerService.GetObserverById(observerid));
        }


        [HttpGet("product/{productid}")]
        public async Task<ActionResult<Response>> GetAllByProductId(int productid, bool notify = false){
            Console.WriteLine("GetAllByProductId called ! ");
            var response = await _observerService.GetObserverByProductId(productid);

            if(notify){
                List<Observer> observerList = (List<Observer>)response.Data;    //convert response to observer list
                //Console.WriteLine(observerList[0].ToString());
                Console.WriteLine("Notify!");
                foreach (var observer in observerList)
                {
                    Console.WriteLine("ID: " + observer.Id);
                    Console.WriteLine("UserId: " + observer.UserId);
                    Console.WriteLine("ProductId: " + observer.ProductId);
                    //notify user with observer.UserID here
                    await NotifyObserver(observer.Id);

                    //Send server sent event / email / push notification to user with userId here by checking if they have an active session
                    //OnSale attribute will be stored in DB regardless so user can see when they go to view their watchlist
                    //PushNotification(obsever.UserId)
                }
            }
            return Ok(response);
        }

        [HttpGet("user/{userid}")]
        public async Task<ActionResult<Response>> GetAllByUserId(int userid){
           // return Ok(observer);
            return Ok(await _observerService.GetObserverByUserId(userid));
        }

        [HttpPost]
        public async Task<ActionResult<Response>> AddObserver(Observer newObserver){
            return Ok(await _observerService.AddObserver(newObserver));
        }

        [HttpDelete("{observerid}")]
        public async Task<ActionResult<Response>> RemoveObserver(int observerid){
            return Ok(await _observerService.RemoveObserver(observerid));
        }

        [HttpPut]
        public async Task<ActionResult<Response>> NotifyObserver(int observerid){
            return Ok(await _observerService.NotifyObserver(observerid));
        }

        //[HttpGet("notify/{ProductId}")]
        // public async void NotifyObservers(int ProductId){
        //     Console.WriteLine("Notifying Observers: " + ProductId);
        //     var list = await this.GetAllByProductId(ProductId);
        //     Console.WriteLine(list.GetType());
        //     Console.WriteLine(list.Value.Data.ToString());
        //     //var list = await _observerService.GetObserverByProductId(ProductId);
        // }
        // [HttpGet("product/{productid}")]
        // public void NotifyObserver(int UserID){
        //     //var observerList = await this.GetAllByProductId(ProductId, true);
        // }
    }
}
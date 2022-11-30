using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.Services.ObserverService
{
    public interface IObserverService
    {
        Task<Response> GetAllObservers();
        Task<Response> GetObserverById(int id);
        Task<Response> GetObserverByProductId(int productid);
        Task<Response> GetObserverByUserId(int userid);
        Task<Response> AddObserver(Observer newObserver);
        Task<Response> RemoveObserver(int observerid);
    }
}
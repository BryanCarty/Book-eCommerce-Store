using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.Data;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Services.ObserverService
{
    public class ObserverService : IObserverService
    {
        private readonly DataContext _context;

        public ObserverService(DataContext context){
            _context = context;

        }

        public async Task<Response> AddObserver(Observer newObserver)
        {
            var serviceResponse = new Response();
            _context.Observers.Add(newObserver);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Observers.ToListAsync();
            return serviceResponse;
        }

        public async Task<Response> RemoveObserver(int observerid)
        {
            var serviceResponse = new Response();

            try{
                Observer observer = await _context.Observers.FirstAsync(c => c.Id == observerid);
                _context.Observers.Remove(observer);
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Observers.ToListAsync();
            }
            catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<Response> GetAllObservers()
        {
            var serviceResponse = new Response();
            var dbObservers = await _context.Observers.ToListAsync();
            serviceResponse.Data = dbObservers.ToList();
            return serviceResponse;
        }

        public async Task<Response> GetObserverById(int observerid)
        {
            var serviceResponse = new Response();
            var dbObserver = await _context.Observers.FirstOrDefaultAsync(c => c.Id == observerid);
            serviceResponse.Data = dbObserver;
            return serviceResponse;
        }

        public async Task<Response> GetObserverByProductId(int productid)
        {
            var serviceResponse = new Response();
            var dbObservers = await _context.Observers.Where(c => c.ProductId == productid).ToListAsync();
            serviceResponse.Data = dbObservers;
            return serviceResponse;
        }

        public async Task<Response> GetObserverByUserId(int userid)
        {
            var serviceResponse = new Response();
            var dbObservers = await _context.Observers.Where(c => c.UserId == userid).ToListAsync();
            serviceResponse.Data = dbObservers;
            return serviceResponse;
        }

        public async Task<Response> NotifyObserver(int observerid)
        {
            var serviceResponse = new Response();
            var dbObserver = await _context.Observers.FirstOrDefaultAsync(c => c.Id == observerid);
            dbObserver.OnSale = true;

            await _context.SaveChangesAsync();
            serviceResponse.Data = dbObserver;
            return serviceResponse;
        }
    }
}
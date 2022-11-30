using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.Data.Entities;

namespace Book_eCommerce_Store.Observers;

public class ProductWatchList : IObserver<PRODUCT>{

    public List<PRODUCT> ProductList;
    public List<WatchListItem>? WatchList;

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
        }

        
        public void OnError(Exception error)
        {
            Console.WriteLine("Error");
        }

        
        public void OnNext(PRODUCT value)
        {
            //notify observers here -> create a server sent event to client. Client webpage will dynamically update UI with notification box stating item is on sale/added
              Console.WriteLine($"Name: {value.Name}, Quantity: {value.Quantity}, Description: {value.Description}, PriceInCents: {value.PriceInCent}, ProductCategory: {value.ProductCategory}");
        }

        // public void AlertSubscription(string Name, int PriceInCent){
        //     Console.WriteLine("ALErt!");
        //    // updateItem(Name, PriceInCent);
        // }

        // public void AlertSubscription(){
        //     Console.WriteLine("ALErt!");
        // }

        public void addItem(string ProductName, int Price){
            WatchList.Add(new WatchListItem(ProductName, Price, false));
        }

        public void removeItem(string ProductName){
            var itemToRemove = WatchList.SingleOrDefault(r => r.Name == ProductName);
            if (itemToRemove != null) WatchList.Remove(itemToRemove);
        }

        public void updateItem(string ProductName, int newPrice){

            var itemToUpdate = WatchList.SingleOrDefault(r => r.Name == ProductName);

            if (itemToUpdate != null) {

                if(itemToUpdate.PriceInCent > newPrice){
                   // itemToUpdate.OnSale = true; //set item to on sale if new price is lower than previous price
                    //notify observers -> update DOM (server sent event)
                }

                itemToUpdate.PriceInCent = newPrice;
            }  
        }

}

public class WatchListItem : ProductWatchList {
    public string Name;
    public int PriceInCent;
    public bool OnSale;
    public WatchListItem(string ProductName, int Price, bool OnSale = false){

        this.Name = ProductName;
        this.PriceInCent = Price;
        this.OnSale = OnSale;
    }

    // public int PriceInCent { get => PriceInCent; set => PriceInCent = value; }

    // public string Name { get => Name; set => Name = value; }

    // public void addItem(string ProductName, int Price){
    //     WatchList.Add(new WatchListItem(ProductName, Price, false));
    // }

    // public void removeItem(string ProductName){
    //     var itemToRemove = WatchList.SingleOrDefault(r => r.Name == ProductName);
    //     if (itemToRemove != null) WatchList.Remove(itemToRemove);
    // }

    // public void updateItem(string ProductName, int newPrice){

    //     var itemToUpdate = WatchList.SingleOrDefault(r => r.Name == ProductName);

    //     if (itemToUpdate != null) {

    //         if(itemToUpdate.PriceInCent > newPrice){
    //             itemToUpdate.OnSale = true; //set item to on sale if new price is lower than previous price
    //             //notify observers
    //         }

    //         itemToUpdate.PriceInCent = newPrice;
    //     }  
    // }

}
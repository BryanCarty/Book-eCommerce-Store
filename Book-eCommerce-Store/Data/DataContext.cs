using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DataContext() { }


        public virtual DbSet<PRODUCT> Products {get; set; }
        public DbSet<Order> Orders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
    }
}
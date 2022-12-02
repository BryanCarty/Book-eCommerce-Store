using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTestProject.Models
{
    public class Observer
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public bool OnSale { get; set; }

    }
}
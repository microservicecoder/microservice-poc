using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ShippingCartItems
    {
        public int Quantity { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }

        //we want to communicate with other service about product, its good to
        //have both Id and Name while sending the info from one service to other.
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}

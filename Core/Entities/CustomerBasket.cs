using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {
            
        }
        public CustomerBasket(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public int? DeliveryMethodId { get; set; }
        public string ClientSecret { get; set; } // This one will be used by Stripe so it can confirm the Payment Intent
        public string PaymentIntentId { get; set; } // This one will be used to update the Payment Intent if clients change their delivery method, order
        public decimal ShippingPrice { get; set; }
    }
}
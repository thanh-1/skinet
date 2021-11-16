using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepo;

        public OrderService(
            IUnitOfWork unitOfWork,
            IBasketRepository basketRepo
        )
        {
            this.unitOfWork = unitOfWork;
            this.basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // Steps to create order:
            // get basket from repo (we do not trust info in the basket at this stage. Ex: the price)
            // get items from product repo
            // get delivery method from repo
            // calculate subtotal
            // create order
            // save to db
            // if successfully save, delete basket
            // return order

            var basket = await this.basketRepo.GetBasketAsync(basketId);
            var items = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var productItem = await this.unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);

                items.Add(orderItem);
            }

            var deliveryMethod = await this.unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            var subTotal = items.Sum(item => item.Price * item.Quantity);
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subTotal);

            this.unitOfWork.Repository<Order>().Add(order);
            var result = await this.unitOfWork.Complete();

            if (result <= 0) return null;

            await this.basketRepo.DeleteBasketAsync(basketId);
            
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {

            return await this.unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
            
            return await this.unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

            return await this.unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}
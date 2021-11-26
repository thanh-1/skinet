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
        private readonly IPaymentService paymentService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IBasketRepository basketRepo,
            IPaymentService paymentService
        )
        {
            this.unitOfWork = unitOfWork;
            this.basketRepo = basketRepo;
            this.paymentService = paymentService;
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

            // Calculate Subtotal
            var subTotal = items.Sum(item => item.Price * item.Quantity);

            // Check to see if order exists
            var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
            var existingOrder = await this.unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            // If we have existing order then we will delete that one
            // If users repeating the action due to payment fail then we get rid of the order completely
            if (existingOrder != null)
            {
                this.unitOfWork.Repository<Order>().Delete(existingOrder);
                await this.paymentService.CreateOrUpdatePaymentIntent(basket.Id);
            }

            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subTotal, basket.PaymentIntentId);

            this.unitOfWork.Repository<Order>().Add(order);
            var result = await this.unitOfWork.Complete();

            if (result <= 0) return null;
            
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
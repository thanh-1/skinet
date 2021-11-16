using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = User.RetreiveEmailFromPrincipal();
            var address = this.mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

            var order = await this.orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

            if (order == null) return BadRequest(new ApiResponse(400, "Problem occured when creating order!"));

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var email = User.RetreiveEmailFromPrincipal();
            var orders = await this.orderService.GetOrdersForUserAsync(email);

            return Ok(this.mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var email = User.RetreiveEmailFromPrincipal();
            var order = await this.orderService.GetOrderByIdAsync(id, email);

            if (order == null) return NotFound(new ApiResponse(404));

            return Ok(this.mapper.Map<Order, OrderToReturnDto>(order));
        }

        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await this.orderService.GetDeliveryMethodsAsync();

            return Ok(deliveryMethods);
        }
    }
}
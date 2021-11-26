using System;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order = Core.Entities.OrderAggregate.Order;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService payment;

        // We will set this to something
        // We will get something from Stripe to tell us that we can trust the thing they send
        // Because the endpoint is available to anybody
        // Only trust things that have this particular secret
        // we will get this webhook secret when we config our enpoint in Stripe
        // To test webhooks, we use Stripe CLI and when it is running, it will give us the secret
        private const string WhSecret = "whsec_Z1wiREkto9vfLl0VDBUJJ8uVtTou9voV";
        private readonly ILogger logger;

        public PaymentsController(IPaymentService payment, ILogger<PaymentsController> logger)
        {
            this.logger = logger;
            this.payment = payment;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await this.payment.CreateOrUpdatePaymentIntent(basketId);
            
            if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket!"));

            return basket;
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            // Read what coming from Stripe to this particular endpoint

            // Get info that Stripe sending us outside the body of the request
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            // Trust this Stripe event by passing the webhook secret
            // This is the part where we confirm the payment from the client
            // and the webhook need to be available anonymously
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    this.logger.LogInformation("Payment Succeeded: ", intent.Id);

                    // TODO: update order with new status
                    order = await this.payment.UpdateOrderPaymentSucceeded(intent.Id);
                    this.logger.LogInformation("Order updated to payment received: ", order.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    this.logger.LogInformation("Payment Failed: ", intent.Id);

                    // Update order status
                    order = await this.payment.UpdateOrderPaymentFailed(intent.Id);
                    this.logger.LogInformation("Payment Failed: ", order.Id);
                    break;
                default:
                    break;
            }
            // Confirm to Stripe that we receive their event
            // In real world, if we do not confirm, Stripe will keep sending the event for 3 days
            return new EmptyResult();
        }
    }
}
import { AfterViewInit, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket } from 'src/app/shared/models/basket';
import { IOrder } from 'src/app/shared/models/order';
import { CheckoutService } from '../checkout.service';

declare var Stripe;

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements AfterViewInit, OnDestroy {

  @Input() checkoutForm: FormGroup;
  @ViewChild('cardNumber', {static: true}) cardNumberElement: ElementRef;
  @ViewChild('cardExpiry', {static: true}) cardExpiryElement: ElementRef;
  @ViewChild('cardCvc', {static: true}) cardCvcElement: ElementRef;

  // Use this to access Stripe javascript functionality
  // We not using Typescript with Stripe, it is pure javascript => data type is any
  stripe: any;
  cardNumber: any;
  cardExpiry: any;
  cardCvc: any;
  cardErrors: any;
  cardHandler = this.onChange.bind(this);

  // Use these flags to disable the payment button until all are set to true
  cardNumberValid = false;
  cardExpiryValid = false;
  cardCvcValid = false;

  loading = false;

  constructor(
    private basketService: BasketService, 
    private checkoutService: CheckoutService, 
    private toastr: ToastrService,
    private route: Router
  ) { }

  ngOnDestroy(): void {
    // When this component is destroyed, destroy Stripe related variables as well
    this.cardNumber.destroy();
    this.cardExpiry.destroy();
    this.cardCvc.destroy();
  }

  ngAfterViewInit(): void { // This give HTML a chance to initialize and then we will mount Stripe elements on top of them
    this.stripe = Stripe('pk_test_51JyUxBHkurOFI2MMcZ10QVMnNT4REyPUaM1CkasSiY97kIpjEhNTTzVHIR5fDIL6h952hdfWjzF0t5XOeP8fD3mT00pHtEx0E5');
    const elements = this.stripe.elements();

    // This is Stripe Elements Functionality
    this.cardNumber = elements.create('cardNumber');
    this.cardNumber.mount(this.cardNumberElement.nativeElement);
    this.cardNumber.addEventListener('change', this.cardHandler);

    this.cardExpiry = elements.create('cardExpiry');
    this.cardExpiry.mount(this.cardExpiryElement.nativeElement);
    this.cardExpiry.addEventListener('change', this.cardHandler);

    this.cardCvc = elements.create('cardCvc');
    this.cardCvc.mount(this.cardCvcElement.nativeElement);
    this.cardCvc.addEventListener('change', this.cardHandler);
  }

  // The {error} is known as destructuring
  // This function will receive an object of some description
  // Inside that object, there will be a property called error which is the one we need
  onChange(event)
  {
    if (event.error) {
      this.cardErrors = event.error.message;
    } else {
      this.cardErrors = null;
    }

    switch (event.elementType) {
      case "cardNumber":
        this.cardNumberValid = event.complete;
        break;
      case "cardExpiry":
        this.cardExpiryValid = event.complete;
        break;
      case "cardCvc":
        this.cardCvcValid = event.complete;
        break;
      default:
        break;
    }
  }

  async submitOrder() // The "async" allow us to use await inside
  {
    this.loading = true;
    const basket = this.basketService.getCurrentBasket();

    try {
      // The await will make this wait until it is finished then move to the next one
      // Each await function return their own error and we can put them in a try catch to get all error
      const createdOrder = await this.createOrder(basket);

      const paymentResult = await this.confirmPaymentWithStripe(basket);

      if (paymentResult.paymentIntent) // If Payment is successed, it will return object called "paymentIntent"
      {
        this.basketService.deleteBasket(basket);
        const navigationExtras: NavigationExtras = {state: createdOrder};
        this.route.navigate(['checkout/success'], navigationExtras);
      } else {
        // Show error coming from Stripe if it has
        this.toastr.error(paymentResult.error.message);
      }
      this.loading = false;
    } catch (error) {
      console.log(error);
      this.loading = false;
    }

    // this.checkoutService.createOrder(orderToCreate).subscribe((order: IOrder) => {

    //   this.stripe.confirmCardPayment(basket.clientSecret, {
    //     payment_method: {
    //       card: this.cardNumber,
    //       billing_details: {
    //         name: this.checkoutForm.get('paymentForm').get('nameOnCard').value
    //       }
    //     }
    //   }).then(result => {
    //     console.log(result);

    //     if (result.paymentIntent) // Payment is successed
    //     {
    //       this.basketService.deleteLocalBasket(basket.id);
    //       const navigationExtras: NavigationExtras = {state: order};
    //       this.route.navigate(['checkout/success'], navigationExtras);
    //     } else {
    //       this.toastr.error(result.error.message);
    //     }
    //   });
    // }, error => {
    //   console.log(error);
    //   this.toastr.error(error.message);
    // });
  }

  // This one return a Promise for a purpose
  private async confirmPaymentWithStripe(basket: IBasket)
  {
    return this.stripe.confirmCardPayment(basket.clientSecret, {
      payment_method: {
        card: this.cardNumber,
        billing_details: {
          name: this.checkoutForm.get('paymentForm').get('nameOnCard').value
        }
      }
    });
  }

  // This one return a Promise for a purpose
  private async createOrder(basket: IBasket)
  {
    const orderToCreate = this.getOrderToCreate(basket);

    // Create Order
    return this.checkoutService.createOrder(orderToCreate).toPromise();
  }

  private getOrderToCreate(basket: IBasket)
  {
    return {
      basketId: basket.id,
      deliveryMethodId: +this.checkoutForm.get('deliveryForm').get('deliveryMethod').value,
      shipToAddress: this.checkoutForm.get('addressForm').value
    }
  }

}

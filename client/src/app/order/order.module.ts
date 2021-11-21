import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderComponent } from './order.component';
import { OrderRoutingModule } from './order-routing.module';
import { OrderDetailedComponent } from './order-detailed/order-detailed.component';



@NgModule({
  declarations: [
    OrderComponent,
    OrderDetailedComponent
  ],
  imports: [
    CommonModule,
    OrderRoutingModule
  ],
  exports: [
    OrderComponent
  ]
})
export class OrderModule { }

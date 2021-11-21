import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';
import { BreadcrumbService } from 'xng-breadcrumb';
import { OrderService } from '../order.service';

@Component({
  selector: 'app-order-detailed',
  templateUrl: './order-detailed.component.html',
  styleUrls: ['./order-detailed.component.scss']
})
export class OrderDetailedComponent implements OnInit {

  order: IOrder;

  constructor(
    private orderService: OrderService, 
    private activeRoute: ActivatedRoute, 
    private breadcrumbService: BreadcrumbService
  ) { 
    this.breadcrumbService.set('@OrderDetailed', '');
  }

  ngOnInit(): void {
    this.loadOrderById();
  }

  loadOrderById()
  {
    return this.orderService.getOrderDetail(+this.activeRoute.snapshot.paramMap.get('id')).subscribe((order: IOrder) => {
      this.order = order;
      this.breadcrumbService.set('@OrderDetailed', `Order #${order.id} - ${order.status}`);
    }, error => {
      console.log(error);
    });
  }

}

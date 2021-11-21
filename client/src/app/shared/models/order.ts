import { IAddress } from "./address";
import { IDeliveryMethod } from "./deliveryMethod";

// This one use to create order
export interface IOrderToCreate {
    basketId: string;
    deliveryMethodId: number;
    shipToAddress: IAddress;
}

// These two use to get order data back from api
export interface IOrder {
    id: number;
    buyerEmail: string;
    orderDate: string;
    shipToAddress: IAddress;
    deliveryMethod: string;
    shippingPrice: number;
    orderItems: IOrderItem[];
    subtotal: number;
    total: number;
    status: string;
}

export interface IOrderItem {
    productId: number;
    productName: string;
    pictureUrl: string;
    price: number;
    quantity: number;
}

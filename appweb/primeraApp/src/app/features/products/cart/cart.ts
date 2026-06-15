import { Component, input, output } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { OrderItem } from './order-item/order-item';

export interface CartLineView {
  code: string;
  name: string;
  imageUrl: string | null;
  unitPrice: number;
  originalPrice: number;
  discountPercent: number;
  quantity: number;
  lineSubtotal: number;
}

@Component({
  selector: 'app-cart',
  imports: [DecimalPipe, OrderItem],
  templateUrl: './cart.html',
})
export class Cart {
  lines = input<CartLineView[]>([]);
  subtotal = input(0);
  count = input(0);

  quantityChange = output<{ code: string; delta: number }>();
  checkout = output<void>();

  onQuantityChange(code: string, delta: number): void {
    this.quantityChange.emit({ code, delta });
  }

  onCheckout(): void {
    this.checkout.emit();
  }
}

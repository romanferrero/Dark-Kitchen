import { Component, input, output } from '@angular/core';
import { DecimalPipe } from '@angular/common';

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
  imports: [DecimalPipe],
  templateUrl: './cart.html',
})
export class Cart {
  lines = input<CartLineView[]>([]);
  subtotal = input(0);
  count = input(0);

  quantityChange = output<{ code: string; delta: number }>();
  checkout = output<void>();

  increase(code: string): void {
    this.quantityChange.emit({ code, delta: 1 });
  }

  decrease(code: string): void {
    this.quantityChange.emit({ code, delta: -1 });
  }

  onCheckout(): void {
    this.checkout.emit();
  }
}

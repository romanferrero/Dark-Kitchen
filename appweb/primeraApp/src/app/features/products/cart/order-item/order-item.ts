import { Component, input, output } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { CartLineView } from '../cart';

@Component({
  selector: 'app-order-item',
  imports: [DecimalPipe],
  templateUrl: './order-item.html',
})
export class OrderItem {
  line = input.required<CartLineView>();

  quantityChange = output<number>();

  increase(): void {
    this.quantityChange.emit(1);
  }

  decrease(): void {
    this.quantityChange.emit(-1);
  }
}

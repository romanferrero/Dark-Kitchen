import { Component, input, output } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { CartLineView } from '../cart';
import { ProductImage } from '../../../../shared/components/product-image/product-image';

@Component({
  selector: 'app-order-item',
  imports: [DecimalPipe, ProductImage],
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

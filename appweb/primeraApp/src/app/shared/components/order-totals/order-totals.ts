import { Component, input } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-order-totals',
  imports: [DecimalPipe],
  templateUrl: './order-totals.html',
})
export class OrderTotals {
  subtotal = input(0);
  shippingCost = input(0);
  tax = input(0);
  total = input(0);
}

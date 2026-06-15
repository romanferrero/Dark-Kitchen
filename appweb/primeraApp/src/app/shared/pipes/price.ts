import { Pipe, PipeTransform } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Pipe({ name: 'price' })
export class PricePipe implements PipeTransform {
  private decimal = new DecimalPipe('en-US');

  transform(value: number | null | undefined): string {
    return `$ ${this.decimal.transform(value ?? 0, '1.2-2')}`;
  }
}

import { Component, input } from '@angular/core';

@Component({
  selector: 'app-product-image',
  templateUrl: './product-image.html',
})
export class ProductImage {
  src = input<string | null | undefined>(null);
  alt = input('');
  imgClass = input('h-full w-full object-cover');
  fallbackClass = input('flex h-full items-center justify-center text-xs text-muted-foreground');
}

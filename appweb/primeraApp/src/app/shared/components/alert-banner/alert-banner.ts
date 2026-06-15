import { Component, input } from '@angular/core';

@Component({
  selector: 'app-alert-banner',
  templateUrl: './alert-banner.html',
})
export class AlertBanner {
  type = input<'success' | 'error'>('error');
  message = input<string | null>(null);
}

import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  template: `<span aria-hidden="true" [class]="classes()"></span>`,
  host: { class: 'contents' },
})
export class Spinner {
  size = input('h-4 w-4');
  variant = input<'muted' | 'on-primary'>('muted');

  classes = computed(
    () =>
      `${this.size()} animate-spin border-2 ` +
      (this.variant() === 'on-primary'
        ? 'border-primary-foreground/25 border-t-primary-foreground/70'
        : 'border-muted-foreground/25 border-t-muted-foreground/70'),
  );
}

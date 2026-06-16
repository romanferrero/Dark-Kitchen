import { Component, computed, input, output } from '@angular/core';
import { Spinner } from '../spinner/spinner';

/**
 * Shared button. Covers the primary/outline variants used across the app and
 * the "spinner + loading text" pattern that was duplicated in every form.
 * Project the label (and any `app-icon`) as content.
 */
@Component({
  selector: 'app-button',
  imports: [Spinner],
  templateUrl: './button.html',
  host: { class: 'block' },
})
export class Button {
  variant = input<'primary' | 'outline'>('primary');
  type = input<'button' | 'submit'>('button');
  size = input<'sm' | 'base'>('sm');
  full = input(false);
  loading = input(false);
  loadingText = input('');
  disabled = input(false);

  clicked = output<void>();

  classes = computed(() => {
    const base =
      'inline-flex items-center justify-center gap-2 px-4 py-2.5 font-medium transition disabled:cursor-not-allowed disabled:opacity-60';
    const size = this.size() === 'base' ? 'text-base' : 'text-sm';
    const full = this.full() ? 'w-full' : '';
    const variant =
      this.variant() === 'outline'
        ? 'border border-border text-foreground hover:bg-muted/40'
        : 'bg-primary text-primary-foreground hover:opacity-90 active:opacity-75';
    return [base, size, full, variant].filter(Boolean).join(' ');
  });
}

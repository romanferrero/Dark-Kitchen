import { Component, computed, input } from '@angular/core';


@Component({
  selector: 'app-badge',
  template: `<span [class]="classes()"><ng-content /></span>`,
})
export class Badge {
  variant = input<'success' | 'info' | 'warning' | 'muted' | 'primary' | 'destructive'>('muted');

  classes = computed(() => {
    const base = 'inline-block px-2 py-0.5 text-xs font-medium border';
    const variants: Record<string, string> = {
      success: 'bg-green-500/15 text-green-600 border-green-500/30',
      info: 'bg-blue-500/15 text-blue-600 border-blue-500/30',
      warning: 'bg-amber-500/15 text-amber-600 border-amber-500/30',
      muted: 'bg-muted text-muted-foreground border-border',
      primary: 'bg-primary/15 text-primary border-transparent',
      destructive: 'bg-destructive/10 text-destructive border-destructive/20',
    };
    return `${base} ${variants[this.variant()]}`;
  });
}

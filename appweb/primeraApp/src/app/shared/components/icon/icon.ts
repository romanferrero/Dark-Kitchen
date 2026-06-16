import { Component, input } from '@angular/core';

export type IconName =
  | 'edit'
  | 'trash'
  | 'plus'
  | 'cart'
  | 'check-circle'
  | 'alert-circle'
  | 'close'
  | 'mail'
  | 'lock'
  | 'phone'
  | 'upload'
  | 'package';

/**
 * Single source of truth for the stroke (feather/lucide) icons used across the
 * app. Use `svgClass` to size/color it; the SVG inherits `currentColor`.
 */
@Component({
  selector: 'app-icon',
  templateUrl: './icon.html',
  host: { class: 'contents' },
})
export class Icon {
  name = input.required<IconName>();
  svgClass = input('w-5 h-5');
  strokeWidth = input(2);
}

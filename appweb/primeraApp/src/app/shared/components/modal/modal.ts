import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.html',
})
export class Modal {
  open = input(false);
  title = input('');
  maxWidth = input('max-w-lg');

  close = output<void>();
}

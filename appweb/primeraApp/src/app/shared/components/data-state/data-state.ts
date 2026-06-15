import { Component, input } from '@angular/core';

@Component({
  selector: 'app-data-state',
  templateUrl: './data-state.html',
})
export class DataState {
  loading = input(false);
  error = input<string | null>(null);
  empty = input(false);
  emptyMessage = input('No items to show.');
}

import { Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-paginator',
  templateUrl: './paginator.html',
})
export class Paginator {
  pageNumber = input(1);
  pageSize = input(10);
  totalCount = input(0);
  totalPages = input(1);

  pageChange = output<number>();

  rangeStart = computed(() =>
    this.totalCount() === 0 ? 0 : (this.pageNumber() - 1) * this.pageSize() + 1,
  );

  rangeEnd = computed(() => Math.min(this.pageNumber() * this.pageSize(), this.totalCount()));

  goTo(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.pageNumber()) {
      return;
    }
    this.pageChange.emit(page);
  }
}

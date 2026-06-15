import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReportService, SalesReport, TopProduct } from '../../core/services/report';
import { AlertBanner } from '../../shared/components/alert-banner/alert-banner';
import { PageHeader } from '../../shared/components/page-header/page-header';
import { Spinner } from '../../shared/components/spinner/spinner';
import { Button } from '../../shared/components/button/button';

type ReportTab = 'sales' | 'top-products';

@Component({
  selector: 'app-reports',
  imports: [ReactiveFormsModule, AlertBanner, PageHeader, Spinner, Button],
  templateUrl: './reports.html',
  styleUrl: './reports.css',
})
export class Reports implements OnInit {
  private fb = inject(FormBuilder);
  private reportService = inject(ReportService);

  activeTab = signal<ReportTab>('sales');

  salesReport = signal<SalesReport | null>(null);
  topProducts = signal<TopProduct[]>([]);

  loading = signal(false);
  errorMessage = signal<string | null>(null);
  topProductsLoaded = signal(false);

  rangeForm = this.fb.group({
    dateFrom: ['', [Validators.required]],
    dateTo: ['', [Validators.required]],
  });

  ngOnInit(): void {
    this.loadSales();
  }

  selectTab(tab: ReportTab): void {
    this.activeTab.set(tab);
    this.errorMessage.set(null);
  }

  loadSales(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.reportService.getSalesReport().subscribe({
      next: (data) => {
        this.salesReport.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Could not load the sales report.');
        this.loading.set(false);
      },
    });
  }

  loadTopProducts(): void {
    if (this.rangeForm.invalid) {
      this.rangeForm.markAllAsTouched();
      return;
    }

    const { dateFrom, dateTo } = this.rangeForm.getRawValue();
    this.loading.set(true);
    this.errorMessage.set(null);

    this.reportService.getTopProducts(dateFrom!, dateTo!).subscribe({
      next: (data) => {
        this.topProducts.set(data);
        this.topProductsLoaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Could not load the products report.');
        this.loading.set(false);
      },
    });
  }

  get dateFrom() {
    return this.rangeForm.controls.dateFrom;
  }
  get dateTo() {
    return this.rangeForm.controls.dateTo;
  }
}

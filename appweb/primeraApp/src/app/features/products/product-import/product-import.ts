import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductImport, ImporterInfo, ImportResult } from '../../../core/services/product-import';
import { AlertBanner } from '../../../shared/components/alert-banner/alert-banner';
import { Icon } from '../../../shared/components/icon/icon';
import { Spinner } from '../../../shared/components/spinner/spinner';
import { Button } from '../../../shared/components/button/button';

@Component({
  selector: 'app-product-import',
  imports: [FormsModule, AlertBanner, Icon, Spinner, Button],
  templateUrl: './product-import.html',
  styleUrl: './product-import.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductImportComponent implements OnInit {
  private importService = inject(ProductImport);

  importers = signal<ImporterInfo[]>([]);
  selectedImporter = signal<ImporterInfo | null>(null);
  paramValues = signal<Record<string, string>>({});
  loading = signal(false);
  loadingImporters = signal(false);
  result = signal<ImportResult | null>(null);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadImporters();
  }

  loadImporters(): void {
    this.loadingImporters.set(true);
    this.importService.getImporters().subscribe({
      next: (data) => {
        this.importers.set(data);
        this.loadingImporters.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load importers');
        this.loadingImporters.set(false);
      },
    });
  }

  onImporterChange(name: string): void {
    const importer = this.importers().find((i) => i.name === name) ?? null;
    this.selectedImporter.set(importer);
    this.result.set(null);
    this.errorMessage.set(null);

    if (importer) {
      const defaults: Record<string, string> = {};
      for (const p of importer.parameters) {
        defaults[p.name] = '';
      }
      this.paramValues.set(defaults);
    } else {
      this.paramValues.set({});
    }
  }

  updateParam(name: string, value: string): void {
    this.paramValues.update((current) => ({ ...current, [name]: value }));
  }

  canSubmit(): boolean {
    const importer = this.selectedImporter();
    if (!importer) return false;

    const values = this.paramValues();
    return importer.parameters
      .filter((p) => p.required)
      .every((p) => values[p.name]?.trim().length > 0);
  }

  onSubmit(): void {
    const importer = this.selectedImporter();
    if (!importer) return;

    this.loading.set(true);
    this.result.set(null);
    this.errorMessage.set(null);

    this.importService
      .importProducts({
        importerName: importer.name,
        parameters: this.paramValues(),
      })
      .subscribe({
        next: (res) => {
          this.result.set(res);
          this.loading.set(false);
        },
        error: (err) => {
          this.errorMessage.set(err.error?.message ?? 'Error importing products');
          this.loading.set(false);
        },
      });
  }
}

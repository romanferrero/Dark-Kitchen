import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { AuditService, AuditLogResponse } from '../../core/services/audit';
import { AlertBanner } from '../../shared/components/alert-banner/alert-banner';
import { PageHeader } from '../../shared/components/page-header/page-header';
import { FormField } from '../../shared/components/form-field/form-field';
import { Button } from '../../shared/components/button/button';

function dateRangeValidator(group: AbstractControl): ValidationErrors | null {
  const dateFrom = group.get('dateFrom')?.value;
  const dateTo = group.get('dateTo')?.value;
  if (dateFrom && dateTo && new Date(dateFrom) >= new Date(dateTo)) {
    return { dateRangeInvalid: true };
  }
  return null;
}

@Component({
  selector: 'app-audit',
  imports: [ReactiveFormsModule, AlertBanner, PageHeader, FormField, Button],
  templateUrl: './audit.html',
  styleUrl: './audit.css',
})
export class Audit {
  private fb = inject(FormBuilder);
  private auditService = inject(AuditService);

  logs = signal<AuditLogResponse[]>([]);
  loading = signal(false);
  searched = signal(false);
  errorMessage = signal<string | null>(null);

  form = this.fb.group(
    {
      dateFrom: ['', [Validators.required]],
      dateTo: ['', [Validators.required]],
      entityName: [''],
      entityId: [null as number | null],
    },
    { validators: dateRangeValidator },
  );

  onSearch(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { dateFrom, dateTo, entityName, entityId } = this.form.getRawValue();

    this.loading.set(true);
    this.errorMessage.set(null);

    this.auditService
      .getAuditLogs({
        dateFrom: dateFrom!,
        dateTo: dateTo!,
        entityName: entityName || undefined,
        entityId: entityId ?? undefined,
      })
      .subscribe({
        next: (data) => {
          this.logs.set(data);
          this.loading.set(false);
          this.searched.set(true);
        },
        error: (err) => {
          this.loading.set(false);
          this.errorMessage.set(err.error?.message ?? 'Error fetching audit records.');
        },
      });
  }

  get dateFrom() {
    return this.form.controls.dateFrom;
  }
  get dateTo() {
    return this.form.controls.dateTo;
  }
  get dateRangeInvalid() {
    return this.form.errors?.['dateRangeInvalid'] && this.form.touched;
  }

  formatTimestamp(ts: string): string {
    return new Date(ts).toLocaleString();
  }
}

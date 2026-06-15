import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DeliveryTypeService, DeliveryTypeResponse } from '../../core/services/delivery-type';
import { Auth } from '../../core/services/auth';
import { AlertBanner } from '../../shared/components/alert-banner/alert-banner';
import { DataState } from '../../shared/components/data-state/data-state';
import { Modal } from '../../shared/components/modal/modal';
import { FormField } from '../../shared/components/form-field/form-field';
import { PageHeader } from '../../shared/components/page-header/page-header';
import { Icon } from '../../shared/components/icon/icon';
import { Button } from '../../shared/components/button/button';

@Component({
  selector: 'app-delivery-types',
  imports: [
    ReactiveFormsModule,
    AlertBanner,
    DataState,
    Modal,
    FormField,
    PageHeader,
    Icon,
    Button,
  ],
  templateUrl: './delivery-types.html',
  styleUrl: './delivery-types.css',
})
export class DeliveryTypes implements OnInit {
  private fb = inject(FormBuilder);
  private deliveryTypeService = inject(DeliveryTypeService);
  private auth = inject(Auth);

  can(permission: string): boolean {
    return this.auth.hasPermission(permission);
  }

  deliveryTypes = signal<DeliveryTypeResponse[]>([]);
  loading = signal(false);
  submitting = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  editingId = signal<number | null>(null);
  showEditModal = signal(false);
  modalErrorMessage = signal<string | null>(null);

  form = this.fb.group({
    name: ['', [Validators.required]],
    shippingCost: [null as number | null, [Validators.required, Validators.min(0)]],
  });

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading.set(true);
    this.deliveryTypeService.getAll().subscribe({
      next: (data) => {
        this.deliveryTypes.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  startEdit(item: DeliveryTypeResponse): void {
    this.editingId.set(item.id);
    this.form.setValue({ name: item.name, shippingCost: item.shippingCost });
    this.modalErrorMessage.set(null);
    this.showEditModal.set(true);
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.form.reset();
    this.modalErrorMessage.set(null);
    this.showEditModal.set(false);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const { name, shippingCost } = this.form.getRawValue();
    const payload = { name: name!, shippingCost: shippingCost! };
    const id = this.editingId();

    const request =
      id !== null
        ? this.deliveryTypeService.update(id, payload)
        : this.deliveryTypeService.create(payload);

    request.subscribe({
      next: () => {
        this.submitting.set(false);
        if (id !== null) {
          this.editingId.set(null);
          this.form.reset();
          this.showEditModal.set(false);
        } else {
          this.successMessage.set('Delivery type created.');
          this.form.reset();
        }
        this.loadAll();
      },
      error: (err) => {
        this.submitting.set(false);
        const msg = err.error?.message ?? 'Could not save. Check entered data.';
        if (id !== null) {
          this.modalErrorMessage.set(msg);
        } else {
          this.errorMessage.set(msg);
        }
      },
    });
  }

  onDelete(id: number): void {
    if (!confirm('Delete this delivery type?')) return;
    this.deliveryTypeService.delete(id).subscribe({
      next: () => this.loadAll(),
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Could not delete.'),
    });
  }

  get name() {
    return this.form.controls.name;
  }
  get shippingCost() {
    return this.form.controls.shippingCost;
  }
}

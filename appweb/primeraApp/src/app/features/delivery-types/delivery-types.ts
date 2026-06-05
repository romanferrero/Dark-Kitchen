import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DeliveryTypeService, DeliveryTypeResponse } from '../../core/services/delivery-type';

@Component({
  selector: 'app-delivery-types',
  imports: [ReactiveFormsModule],
  templateUrl: './delivery-types.html',
  styleUrl: './delivery-types.css',
})
export class DeliveryTypes implements OnInit {
  private fb = inject(FormBuilder);
  private deliveryTypeService = inject(DeliveryTypeService);

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
          this.successMessage.set('Tipo de envío creado.');
          this.form.reset();
        }
        this.loadAll();
      },
      error: (err) => {
        this.submitting.set(false);
        const msg = err.error?.message ?? 'No se pudo guardar. Revisá los datos ingresados.';
        if (id !== null) {
          this.modalErrorMessage.set(msg);
        } else {
          this.errorMessage.set(msg);
        }
      },
    });
  }

  onDelete(id: number): void {
    if (!confirm('¿Eliminar este tipo de envío?')) return;
    this.deliveryTypeService.delete(id).subscribe({
      next: () => this.loadAll(),
      error: (err) => this.errorMessage.set(err.error?.message ?? 'No se pudo eliminar.'),
    });
  }

  get name() {
    return this.form.controls.name;
  }
  get shippingCost() {
    return this.form.controls.shippingCost;
  }
}

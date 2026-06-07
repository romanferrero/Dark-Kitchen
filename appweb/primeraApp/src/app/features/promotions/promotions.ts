import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  PromotionService,
  PromotionResponse,
  PromotionFilters,
} from '../../core/services/promotion';
import { ProductService, ProductResponse } from '../../core/services/product';

@Component({
  selector: 'app-promotions',
  imports: [ReactiveFormsModule],
  templateUrl: './promotions.html',
  styleUrl: './promotions.css',
})
export class Promotions implements OnInit {
  private fb = inject(FormBuilder);
  private promotionService = inject(PromotionService);
  private productService = inject(ProductService);

  promotions = signal<PromotionResponse[]>([]);
  products = signal<ProductResponse[]>([]);

  loading = signal(false);
  submitting = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // edición
  editingId = signal<number | null>(null);
  showEditModal = signal(false);
  modalErrorMessage = signal<string | null>(null);

  // gestión de productos
  managing = signal<PromotionResponse | null>(null);
  productsModalError = signal<string | null>(null);
  selectedProductCode = signal<string>('');

  // productos que todavía no están en la promoción que se está gestionando
  availableProducts = computed(() => {
    const promo = this.managing();
    if (!promo) return [];
    const used = new Set(promo.products);
    return this.products().filter((p) => !used.has(p.code));
  });

  filterForm = this.fb.group({
    date: [''],
    line: [''],
    product: [''],
  });

  form = this.fb.group({
    name: ['', [Validators.required]],
    discount: [
      null as number | null,
      [Validators.required, Validators.min(1), Validators.max(100)],
    ],
    dateFrom: ['', [Validators.required]],
    dateTo: ['', [Validators.required]],
  });

  ngOnInit(): void {
    this.loadAll();
    this.productService.getAll().subscribe({
      next: (data) => this.products.set(data),
    });
  }

  loadAll(): void {
    this.loading.set(true);
    const filters: PromotionFilters = this.filterForm.getRawValue();
    this.promotionService.getAll(filters).subscribe({
      next: (data) => {
        this.promotions.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  clearFilters(): void {
    this.filterForm.reset({ date: '', line: '', product: '' });
    this.loadAll();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const { name, discount, dateFrom, dateTo } = this.form.getRawValue();
    const payload = { name: name!, discount: discount!, dateFrom: dateFrom!, dateTo: dateTo! };
    const id = this.editingId();

    const request =
      id !== null
        ? this.promotionService.update(id, payload)
        : this.promotionService.create(payload);

    request.subscribe({
      next: () => {
        this.submitting.set(false);
        if (id !== null) {
          this.cancelEdit();
        } else {
          this.successMessage.set('Promoción creada.');
          this.form.reset({ name: '', discount: null, dateFrom: '', dateTo: '' });
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

  startEdit(promo: PromotionResponse): void {
    this.editingId.set(promo.id);
    this.form.setValue({
      name: promo.name,
      discount: promo.discountPercentage,
      dateFrom: promo.dateFrom,
      dateTo: promo.dateTo,
    });
    this.modalErrorMessage.set(null);
    this.showEditModal.set(true);
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.form.reset({ name: '', discount: null, dateFrom: '', dateTo: '' });
    this.modalErrorMessage.set(null);
    this.showEditModal.set(false);
  }

  openProducts(promo: PromotionResponse): void {
    this.managing.set(promo);
    this.selectedProductCode.set('');
    this.productsModalError.set(null);
  }

  closeProducts(): void {
    this.managing.set(null);
    this.selectedProductCode.set('');
    this.productsModalError.set(null);
  }

  addProduct(): void {
    const promo = this.managing();
    const code = this.selectedProductCode();
    if (!promo || !code) return;

    this.productsModalError.set(null);
    this.promotionService.addProduct(promo.id, code).subscribe({
      next: () => {
        const updated = { ...promo, products: [...promo.products, code] };
        this.managing.set(updated);
        this.selectedProductCode.set('');
        this.syncPromotion(updated);
      },
      error: (err) => {
        this.productsModalError.set(err.error?.message ?? 'No se pudo agregar el producto.');
      },
    });
  }

  removeProduct(code: string): void {
    const promo = this.managing();
    if (!promo) return;

    this.productsModalError.set(null);
    this.promotionService.removeProduct(promo.id, code).subscribe({
      next: () => {
        const updated = { ...promo, products: promo.products.filter((c) => c !== code) };
        this.managing.set(updated);
        this.syncPromotion(updated);
      },
      error: (err) => {
        this.productsModalError.set(err.error?.message ?? 'No se pudo quitar el producto.');
      },
    });
  }

  // mantiene la fila de la tabla en sincronía con los cambios del modal
  private syncPromotion(updated: PromotionResponse): void {
    this.promotions.update((list) => list.map((p) => (p.id === updated.id ? updated : p)));
  }

  productName(code: string): string {
    return this.products().find((p) => p.code === code)?.name ?? code;
  }

  get name() {
    return this.form.controls.name;
  }
  get discount() {
    return this.form.controls.discount;
  }
  get dateFrom() {
    return this.form.controls.dateFrom;
  }
  get dateTo() {
    return this.form.controls.dateTo;
  }
}

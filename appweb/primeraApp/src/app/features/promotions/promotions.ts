import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  PromotionService,
  PromotionResponse,
  PromotionFilters,
} from '../../core/services/promotion';
import { ProductService, ProductResponse } from '../../core/services/product';
import { Auth } from '../../core/services/auth';
import { Paginator } from '../../shared/components/paginator/paginator';

@Component({
  selector: 'app-promotions',
  imports: [ReactiveFormsModule, Paginator],
  templateUrl: './promotions.html',
  styleUrl: './promotions.css',
})
export class Promotions implements OnInit {
  private fb = inject(FormBuilder);
  private promotionService = inject(PromotionService);
  private productService = inject(ProductService);
  private auth = inject(Auth);

  can(permission: string): boolean {
    return this.auth.hasPermission(permission);
  }

  promotions = signal<PromotionResponse[]>([]);
  products = signal<ProductResponse[]>([]);

  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = signal(1);

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
    // El cliente (sin permiso de gestión) arranca viendo solo las promociones vigentes hoy.
    if (!this.can('ManagePromotions')) {
      this.filterForm.patchValue({ date: this.today() });
    }

    this.loadAll();
    this.productService.getAll().subscribe({
      next: (data) => this.products.set(data.items),
    });
  }

  private today(): string {
    return new Date().toISOString().split('T')[0];
  }

  promotionStatus(promo: PromotionResponse): 'active' | 'upcoming' | 'expired' {
    const today = this.today();
    if (today < promo.dateFrom) return 'upcoming';
    if (today > promo.dateTo) return 'expired';
    return 'active';
  }

  loadAll(): void {
    this.loading.set(true);
    const filters: PromotionFilters = {
      ...this.filterForm.getRawValue(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
    this.promotionService.getAll(filters).subscribe({
      next: (data) => {
        this.promotions.set(data.items);
        this.totalCount.set(data.totalCount);
        this.totalPages.set(data.totalPages);
        this.pageNumber.set(data.pageNumber);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadAll();
  }

  clearFilters(): void {
    this.filterForm.reset({ date: '', line: '', product: '' });
    this.pageNumber.set(1);
    this.loadAll();
  }

  goToPage(page: number): void {
    this.pageNumber.set(page);
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
          this.successMessage.set('Promotion created.');
          this.form.reset({ name: '', discount: null, dateFrom: '', dateTo: '' });
        }
        this.loadAll();
      },
      error: (err) => {
        this.submitting.set(false);
        const msg = err.error?.message ?? 'Could not save. Check the entered data.';
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
        this.productsModalError.set(err.error?.message ?? 'Could not add the product.');
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
        this.productsModalError.set(err.error?.message ?? 'Could not remove the product.');
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

import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ProductService, ProductResponse } from '../../core/services/product';
import { OrderService, CreateOrderResult } from '../../core/services/order';
import { DeliveryTypeService, DeliveryTypeResponse } from '../../core/services/delivery-type';
import { PromotionService, PromotionResponse } from '../../core/services/promotion';
import { Auth } from '../../core/services/auth';
import { Paginator } from '../../shared/components/paginator/paginator';
import { AlertBanner } from '../../shared/components/alert-banner/alert-banner';
import { DataState } from '../../shared/components/data-state/data-state';
import { Modal } from '../../shared/components/modal/modal';
import { FormField } from '../../shared/components/form-field/form-field';
import { PageHeader } from '../../shared/components/page-header/page-header';
import { Badge } from '../../shared/components/badge/badge';
import { Icon } from '../../shared/components/icon/icon';
import { Button } from '../../shared/components/button/button';
import { ProductImage } from '../../shared/components/product-image/product-image';
import { OrderTotals } from '../../shared/components/order-totals/order-totals';
import { Cart, CartLineView } from './cart/cart';

interface CartItem {
  product: ProductResponse;
  quantity: number;
}

const JPEG_DATA_URI_PREFIX = 'data:image/jpeg;base64,';
const MAX_IMAGE_BYTES = 500 * 1024;

function base64ImagesValidator(control: AbstractControl): ValidationErrors | null {
  const value = (control.value ?? '').toString();
  const entries = value
    .split('\n')
    .map((e: string) => e.trim())
    .filter((e: string) => e.length > 0);
  if (entries.length < 1 || entries.length > 3) return { imageCount: true };

  const allJpeg = entries.every((e: string) => e.startsWith(JPEG_DATA_URI_PREFIX));
  return allJpeg ? null : { imageFormat: true };
}

@Component({
  selector: 'app-products',
  imports: [
    ReactiveFormsModule,
    DecimalPipe,
    Paginator,
    Cart,
    AlertBanner,
    DataState,
    Modal,
    FormField,
    PageHeader,
    Badge,
    Icon,
    Button,
    ProductImage,
    OrderTotals,
  ],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private orderService = inject(OrderService);
  private deliveryTypeService = inject(DeliveryTypeService);
  private promotionService = inject(PromotionService);
  private auth = inject(Auth);

  products = signal<ProductResponse[]>([]);
  pageNumber = signal(1);
  pageSize = signal(12);
  totalCount = signal(0);
  totalPages = signal(1);
  loading = signal(false);
  submitting = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  modalErrorMessage = signal<string | null>(null);
  showModal = signal(false);
  editingId = signal<number | null>(null);
  selectedImages = signal<string[]>([]);

  // Tienda del cliente (catálogo + carrito + checkout)
  isShopper = computed(() => !this.auth.hasPermission('ManageProducts'));
  deliveryTypes = signal<DeliveryTypeResponse[]>([]);
  activePromotions = signal<PromotionResponse[]>([]);
  cart = signal<CartItem[]>([]);
  showCheckout = signal(false);
  creating = signal(false);
  createError = signal<string | null>(null);
  showReceipt = signal(false);
  lastResult = signal<CreateOrderResult | null>(null);

  checkoutForm = this.fb.group({
    deliveryType: ['', [Validators.required]],
    street: ['', [Validators.required]],
    doorNumber: ['', [Validators.required]],
    apartment: [''],
  });

  can(permission: string): boolean {
    return this.auth.hasPermission(permission);
  }

  filterForm = this.fb.group({
    name: [''],
    line: [''],
    category: [''],
  });

  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(50)]],
    price: [null as number | null, [Validators.required, Validators.min(0)]],
    description: ['', [Validators.required, Validators.minLength(20), Validators.maxLength(500)]],
    line: ['', [Validators.required]],
    category: ['', [Validators.required]],
    images: ['', [Validators.required, base64ImagesValidator]],
    active: [true],
  });

  ngOnInit(): void {
    this.loadAll();
    if (this.isShopper()) {
      this.deliveryTypeService.getAll().subscribe({ next: (d) => this.deliveryTypes.set(d) });
      this.promotionService
        .getAll({ date: this.todayDate() })
        .subscribe({ next: (d) => this.activePromotions.set(d.items) });
    }
  }

  loadAll(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    const { name, line, category } = this.filterForm.getRawValue();
    this.productService
      .getAll({
        name: name ?? '',
        line: line ?? '',
        categories: category ?? '',
        pageNumber: this.pageNumber(),
        pageSize: this.pageSize(),
      })
      .subscribe({
        next: (data) => {
          this.products.set(data.items);
          this.totalCount.set(data.totalCount);
          this.totalPages.set(data.totalPages);
          this.pageNumber.set(data.pageNumber);
          this.loading.set(false);
        },
        error: (err) => {
          this.errorMessage.set(err.error?.message ?? 'Could not load products.');
          this.loading.set(false);
        },
      });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadAll();
  }

  clearFilters(): void {
    this.filterForm.reset({ name: '', line: '', category: '' });
    this.pageNumber.set(1);
    this.loadAll();
  }

  goToPage(page: number): void {
    this.pageNumber.set(page);
    this.loadAll();
  }

  private todayDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  // Mayor descuento entre las promociones vigentes que incluyen el producto (0 si no tiene).
  discountFor(product: ProductResponse): number {
    const discounts = this.activePromotions()
      .filter((promo) => promo.products.includes(product.code))
      .map((promo) => promo.discountPercentage);
    return discounts.length > 0 ? Math.max(...discounts) : 0;
  }

  discountedPrice(product: ProductResponse): number {
    return product.price * (1 - this.discountFor(product) / 100);
  }

  addToCart(product: ProductResponse): void {
    this.cart.update((items) => {
      const existing = items.find((i) => i.product.code === product.code);
      if (existing) {
        return items.map((i) =>
          i.product.code === product.code ? { ...i, quantity: i.quantity + 1 } : i,
        );
      }
      return [...items, { product, quantity: 1 }];
    });
  }

  changeQuantity(code: string, delta: number): void {
    this.cart.update((items) =>
      items
        .map((i) => (i.product.code === code ? { ...i, quantity: i.quantity + delta } : i))
        .filter((i) => i.quantity > 0),
    );
  }

  removeFromCart(code: string): void {
    this.cart.update((items) => items.filter((i) => i.product.code !== code));
  }

  lineSubtotal(item: CartItem): number {
    return this.discountedPrice(item.product) * item.quantity;
  }

  cartSubtotal = computed(() =>
    this.cart().reduce((sum, item) => sum + this.discountedPrice(item.product) * item.quantity, 0),
  );

  cartCount = computed(() => this.cart().reduce((sum, item) => sum + item.quantity, 0));

  cartLines = computed<CartLineView[]>(() =>
    this.cart().map((item) => ({
      code: item.product.code,
      name: item.product.name,
      imageUrl: item.product.imageUrls.length > 0 ? item.product.imageUrls[0] : null,
      unitPrice: this.discountedPrice(item.product),
      originalPrice: item.product.price,
      discountPercent: this.discountFor(item.product),
      quantity: item.quantity,
      lineSubtotal: this.lineSubtotal(item),
    })),
  );

  openCheckout(): void {
    if (this.cart().length === 0) return;
    this.createError.set(null);
    this.checkoutForm.reset({ deliveryType: '', street: '', doorNumber: '', apartment: '' });
    this.showCheckout.set(true);
  }

  closeCheckout(): void {
    this.showCheckout.set(false);
  }

  submitOrder(): void {
    this.checkoutForm.markAllAsTouched();
    if (this.checkoutForm.invalid || this.cart().length === 0) return;

    const clientId = this.auth.getUserId();
    if (clientId === null) {
      this.createError.set('Could not identify the client. Please log in again.');
      return;
    }

    this.creating.set(true);
    this.createError.set(null);

    const raw = this.checkoutForm.getRawValue();
    this.orderService
      .create({
        clientId,
        deliveryType: raw.deliveryType!,
        street: raw.street!,
        doorNumber: raw.doorNumber!,
        apartment: raw.apartment ?? '',
        products: this.cart().map((i) => ({ productCode: i.product.code, quantity: i.quantity })),
      })
      .subscribe({
        next: (res) => {
          this.creating.set(false);
          this.showCheckout.set(false);
          this.lastResult.set(res);
          this.showReceipt.set(true);
        },
        error: (err) => {
          this.creating.set(false);
          this.createError.set(err.error?.message ?? 'Could not create the order. Check the data.');
        },
      });
  }

  closeReceipt(): void {
    this.showReceipt.set(false);
    this.lastResult.set(null);
    this.cart.set([]);
  }

  openCreate(): void {
    this.editingId.set(null);
    this.form.reset({ active: true } as never);
    this.selectedImages.set([]);
    this.modalErrorMessage.set(null);
    this.showModal.set(true);
  }

  startEdit(item: ProductResponse): void {
    this.editingId.set(item.id);
    // Solo imágenes base64 (las viejas URLs se descartan) y sin duplicados.
    const validImages = [
      ...new Set(item.imageUrls.filter((url) => url.startsWith(JPEG_DATA_URI_PREFIX))),
    ];
    this.selectedImages.set(validImages);
    this.form.reset({
      name: item.name,
      price: item.price,
      description: item.description,
      line: item.line,
      category: item.category,
      images: validImages.join('\n'),
      active: item.active,
    } as never);
    this.modalErrorMessage.set(null);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId.set(null);
    this.form.reset({ active: true } as never);
    this.selectedImages.set([]);
    this.modalErrorMessage.set(null);
  }

  async onFilesSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    input.value = '';
    this.modalErrorMessage.set(null);

    for (const file of files) {
      if (file.type !== 'image/jpeg') {
        this.modalErrorMessage.set('Only JPEG images are allowed.');
        continue;
      }
      if (file.size > MAX_IMAGE_BYTES) {
        this.modalErrorMessage.set('Each image must be 500kb or less.');
        continue;
      }
      const dataUri = await this.readAsDataUri(file);
      // Tope de 3 y sin duplicados, de forma atómica (evita carreras del FileReader async).
      this.selectedImages.update((images) => {
        if (images.length >= 3) {
          this.modalErrorMessage.set('Up to 3 images allowed.');
          return images;
        }
        if (images.includes(dataUri)) {
          this.modalErrorMessage.set('That image is already added.');
          return images;
        }
        return [...images, dataUri];
      });
    }

    this.syncImagesControl();
  }

  removeImage(index: number): void {
    this.selectedImages.update((images) => images.filter((_, i) => i !== index));
    this.syncImagesControl();
  }

  private syncImagesControl(): void {
    this.images.setValue(this.selectedImages().join('\n'));
    this.images.markAsTouched();
  }

  private readAsDataUri(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = () => reject(reader.error);
      reader.readAsDataURL(file);
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.modalErrorMessage.set(null);

    const raw = this.form.getRawValue();
    const payload = {
      name: raw.name!,
      price: raw.price!,
      description: raw.description!,
      line: raw.line!,
      category: raw.category!,
      images: raw.images!,
      active: raw.active ?? true,
    };

    const id = this.editingId();
    const request =
      id !== null ? this.productService.update(id, payload) : this.productService.create(payload);

    request.subscribe({
      next: () => {
        this.submitting.set(false);
        this.successMessage.set(id !== null ? 'Product updated.' : 'Product created.');
        this.closeModal();
        this.loadAll();
      },
      error: (err) => {
        this.submitting.set(false);
        this.modalErrorMessage.set(err.error?.message ?? 'Could not save. Check entered data.');
      },
    });
  }

  get name() {
    return this.form.controls.name;
  }
  get price() {
    return this.form.controls.price;
  }
  get description() {
    return this.form.controls.description;
  }
  get line() {
    return this.form.controls.line;
  }
  get category() {
    return this.form.controls.category;
  }
  get images() {
    return this.form.controls.images;
  }

  priceError = computed(() => {
    if (!(this.price.touched && this.price.invalid)) return null;
    if (this.price.errors?.['required']) return 'Price is required.';
    if (this.price.errors?.['min']) return 'Price cannot be negative.';
    return null;
  });

  imagesError = computed(() => {
    if (!(this.images.touched && this.images.invalid)) return null;
    const errors = this.images.errors;
    if (errors?.['required'] || errors?.['imageCount'])
      return 'You must add between 1 and 3 images.';
    if (errors?.['imageFormat']) return 'All images must be JPEG.';
    return null;
  });
}

import { Component, OnInit, computed, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ProductService, ProductResponse } from '../../core/services/product';
import { Auth } from '../../core/services/auth';

function jpgImagesValidator(control: AbstractControl): ValidationErrors | null {
  const value = (control.value ?? '').toString().trim();
  if (!value) return null;

  const entries = value
    .split(',')
    .map((e: string) => e.trim())
    .filter((e: string) => e.length > 0);
  if (entries.length < 1 || entries.length > 3) return { imageCount: true };

  const allJpg = entries.every((e: string) =>
    e.split('|')[0].trim().toLowerCase().endsWith('.jpg'),
  );
  return allJpg ? null : { imageFormat: true };
}

@Component({
  selector: 'app-products',
  imports: [ReactiveFormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private auth = inject(Auth);

  products = signal<ProductResponse[]>([]);
  loading = signal(false);
  submitting = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  modalErrorMessage = signal<string | null>(null);
  showModal = signal(false);
  editingId = signal<number | null>(null);

  isAdmin = computed(() => this.auth.role() === 'Admin');

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
    images: ['', [Validators.required, jpgImagesValidator]],
    active: [true],
  });

  ngOnInit(): void {
    this.loadAll();
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
      })
      .subscribe({
        next: (data) => {
          this.products.set(data);
          this.loading.set(false);
        },
        error: (err) => {
          this.errorMessage.set(err.error?.message ?? 'Could not load products.');
          this.loading.set(false);
        },
      });
  }

  applyFilters(): void {
    this.loadAll();
  }

  clearFilters(): void {
    this.filterForm.reset({ name: '', line: '', category: '' });
    this.loadAll();
  }

  openCreate(): void {
    this.editingId.set(null);
    this.form.reset({ active: true } as never);
    this.modalErrorMessage.set(null);
    this.showModal.set(true);
  }

  startEdit(item: ProductResponse): void {
    this.editingId.set(item.id);
    this.form.reset({
      name: item.name,
      price: item.price,
      description: item.description,
      line: item.line,
      category: item.category,
      images: item.imageUrls.join(', '),
      active: item.active,
    } as never);
    this.modalErrorMessage.set(null);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId.set(null);
    this.form.reset({ active: true } as never);
    this.modalErrorMessage.set(null);
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
        this.modalErrorMessage.set(
          err.error?.message ?? 'Could not save. Check entered data.',
        );
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
}

import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrderService, OrderSummary, OrderDetail } from '../../core/services/order';
import { ProductService, ProductResponse } from '../../core/services/product';
import { DeliveryTypeService, DeliveryTypeResponse } from '../../core/services/delivery-type';
import { Auth } from '../../core/services/auth';

interface CartItem {
  product: ProductResponse;
  quantity: number;
}

// maquinad e estados
const TRANSITIONS: Record<string, string[]> = {
  Pending: ['Prepared', 'Cancelled', 'Delayed'],
  Delayed: ['Prepared', 'Cancelled'],
  Prepared: ['OnTheWay'],
  OnTheWay: ['Delivered', 'NotDelivered'],
  Cancelled: [],
  Delivered: [],
  NotDelivered: [],
};

//las acciones de cada rol
const ROLE_ACTIONS: Record<string, string[]> = {
  Admin: ['Prepared', 'Cancelled', 'Delayed'],
  Dispatcher: ['Prepared', 'Delayed', 'OnTheWay', 'Delivered', 'NotDelivered'],
  Client: [],
};

const STATUS_LABELS: Record<string, string> = {
  Pending: 'Pendiente',
  Prepared: 'Preparado',
  Delayed: 'Demorado',
  OnTheWay: 'En camino',
  Delivered: 'Entregado',
  Cancelled: 'Cancelado',
  NotDelivered: 'No entregado',
};

const STATUS_BADGE: Record<string, string> = {
  Pending: 'bg-yellow-500/15 text-yellow-500',
  Prepared: 'bg-blue-500/15 text-blue-500',
  Delayed: 'bg-orange-500/15 text-orange-500',
  OnTheWay: 'bg-purple-500/15 text-purple-500',
  Delivered: 'bg-green-500/15 text-green-500',
  Cancelled: 'bg-red-500/15 text-red-500',
  NotDelivered: 'bg-gray-500/15 text-gray-400',
};

const ACTION_LABELS: Record<string, string> = {
  Prepared: 'Preparar',
  Cancelled: 'Cancelar',
  Delayed: 'Marcar demorado',
  OnTheWay: 'Despachar',
  Delivered: 'Entregado',
  NotDelivered: 'No entregado',
};

@Component({
  selector: 'app-orders',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './orders.html',
  styleUrl: './orders.css',
})
export class Orders implements OnInit {
  private fb = inject(FormBuilder);
  private orderService = inject(OrderService);
  private productService = inject(ProductService);
  private deliveryTypeService = inject(DeliveryTypeService);
  private auth = inject(Auth);

  readonly statusOptions = Object.keys(STATUS_LABELS);

  role = computed(() => this.auth.role());
  isClient = computed(() => this.role() === 'Client');
  isDispatcher = computed(() => this.role() === 'Dispatcher');
  isAdmin = computed(() => this.role() === 'Admin');

  orders = signal<OrderSummary[]>([]);
  loading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  statusUpdatingId = signal<number | null>(null);

  detail = signal<OrderDetail | null>(null);
  showDetail = signal(false);
  detailLoading = signal(false);

  lookupId = signal<string>('');

  showCreate = signal(false);
  catalog = signal<ProductResponse[]>([]);
  deliveryTypes = signal<DeliveryTypeResponse[]>([]);
  cart = signal<CartItem[]>([]);
  selectedProductCode = signal<string>('');
  addQuantity = signal<number>(1);
  creating = signal(false);
  createError = signal<string | null>(null);

  filterForm = this.fb.group({
    from: [''],
    to: [''],
    status: [''],
    street: [''],
  });

  createForm = this.fb.group({
    deliveryType: ['', [Validators.required]],
    street: ['', [Validators.required]],
    doorNumber: ['', [Validators.required]],
    apartment: [''],
  });

  ngOnInit(): void {
    if (this.isDispatcher()) {
      const now = new Date();
      const weekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
      this.filterForm.patchValue({
        from: this.toLocalInput(weekAgo),
        to: this.toLocalInput(now),
      });
    }

    if (!this.isAdmin()) {
      this.loadOrders();
    }
  }

  loadOrders(): void {
    const { from, to, status, street } = this.filterForm.getRawValue();

    if (this.isDispatcher() && (!from || !to)) {
      this.errorMessage.set('Como despachante debés indicar el rango de fechas (desde y hasta).');
      this.orders.set([]);
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    this.orderService.getAll({ from, to, status, street }).subscribe({
      next: (data) => {
        this.orders.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'No se pudieron cargar los pedidos.');
        this.loading.set(false);
      },
    });
  }

  clearFilters(): void {
    this.filterForm.reset({ from: '', to: '', status: '', street: '' });
    if (!this.isAdmin()) this.loadOrders();
  }

  openDetail(orderId: number): void {
    this.detail.set(null);
    this.showDetail.set(true);
    this.detailLoading.set(true);
    this.errorMessage.set(null);

    this.orderService.getById(orderId).subscribe({
      next: (data) => {
        this.detail.set(data);
        this.detailLoading.set(false);
      },
      error: (err) => {
        this.detailLoading.set(false);
        this.showDetail.set(false);
        this.errorMessage.set(err.error?.message ?? `No se pudo cargar el pedido ${orderId}.`);
      },
    });
  }

  closeDetail(): void {
    this.showDetail.set(false);
    this.detail.set(null);
  }

  lookupOrder(): void {
    const id = Number(this.lookupId());
    if (!Number.isFinite(id) || id <= 0) {
      this.errorMessage.set('Ingresá un número de pedido (ID) válido.');
      return;
    }
    this.openDetail(id);
  }

  actionsFor(status: string): string[] {
    const roleName = this.role() ?? '';
    const allowed = ROLE_ACTIONS[roleName] ?? [];
    return (TRANSITIONS[status] ?? []).filter((a) => allowed.includes(a));
  }

  updateStatus(orderId: number, action: string): void {
    this.statusUpdatingId.set(orderId);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.orderService.updateStatus(orderId, action).subscribe({
      next: (res) => {
        this.statusUpdatingId.set(null);
        this.successMessage.set(`Pedido actualizado a "${this.statusLabel(res.status)}".`);

        if (this.showDetail() && this.detail()?.orderId === orderId) {
          this.openDetail(orderId);
        }
        if (!this.isAdmin()) {
          this.loadOrders();
        }
      },
      error: (err) => {
        this.statusUpdatingId.set(null);
        this.errorMessage.set(err.error?.message ?? 'No se pudo actualizar el estado del pedido.');
      },
    });
  }

  openCreate(): void {
    this.createError.set(null);
    this.cart.set([]);
    this.selectedProductCode.set('');
    this.addQuantity.set(1);
    this.createForm.reset({ deliveryType: '', street: '', doorNumber: '', apartment: '' });
    this.showCreate.set(true);

    this.productService.getAll().subscribe({
      next: (data) => this.catalog.set(data),
      error: () => this.createError.set('No se pudo cargar el catálogo de productos.'),
    });
    this.deliveryTypeService.getAll().subscribe({
      next: (data) => this.deliveryTypes.set(data),
      error: () => this.createError.set('No se pudieron cargar los tipos de envío.'),
    });
  }

  closeCreate(): void {
    this.showCreate.set(false);
  }

  addToCart(): void {
    const code = this.selectedProductCode();
    const qty = Math.trunc(this.addQuantity());
    if (!code || qty < 1) return;

    const product = this.catalog().find((p) => p.code === code);
    if (!product) return;

    this.cart.update((items) => {
      const existing = items.find((i) => i.product.code === code);
      if (existing) {
        return items.map((i) =>
          i.product.code === code ? { ...i, quantity: i.quantity + qty } : i,
        );
      }
      return [...items, { product, quantity: qty }];
    });

    this.selectedProductCode.set('');
    this.addQuantity.set(1);
  }

  removeFromCart(code: string): void {
    this.cart.update((items) => items.filter((i) => i.product.code !== code));
  }

  cartSubtotal = computed(() =>
    this.cart().reduce((sum, item) => sum + item.product.price * item.quantity, 0),
  );

  submitOrder(): void {
    this.createForm.markAllAsTouched();
    if (this.createForm.invalid) return;

    if (this.cart().length === 0) {
      this.createError.set('Agregá al menos un producto al pedido.');
      return;
    }

    const clientId = this.auth.getUserId();
    if (clientId === null) {
      this.createError.set('No se pudo identificar al cliente. Volvé a iniciar sesión.');
      return;
    }

    this.creating.set(true);
    this.createError.set(null);

    const raw = this.createForm.getRawValue();
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
          this.showCreate.set(false);
          this.successMessage.set(
            `Pedido #${res.orderNumber} creado. Total: $ ${res.total.toFixed(2)}.`,
          );
          this.loadOrders();
        },
        error: (err) => {
          this.creating.set(false);
          this.createError.set(
            err.error?.message ?? 'No se pudo crear el pedido. Revisá los datos.',
          );
        },
      });
  }

  statusLabel(status: string): string {
    return STATUS_LABELS[status] ?? status;
  }

  statusBadge(status: string): string {
    return STATUS_BADGE[status] ?? 'bg-muted text-muted-foreground';
  }

  actionLabel(action: string): string {
    return ACTION_LABELS[action] ?? action;
  }

  private toLocalInput(date: Date): string {
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }
}

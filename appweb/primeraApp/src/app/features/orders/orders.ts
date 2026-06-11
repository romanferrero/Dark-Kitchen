import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { OrderService, OrderSummary, OrderDetail } from '../../core/services/order';
import { Auth } from '../../core/services/auth';

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
  Pending: 'Pending',
  Prepared: 'Prepared',
  Delayed: 'Delayed',
  OnTheWay: 'On the way',
  Delivered: 'Delivered',
  Cancelled: 'Cancelled',
  NotDelivered: 'Not delivered',
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
  Prepared: 'Prepare',
  Cancelled: 'Cancel',
  Delayed: 'Mark delayed',
  OnTheWay: 'Dispatch',
  Delivered: 'Delivered',
  NotDelivered: 'Not delivered',
};

@Component({
  selector: 'app-orders',
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe],
  templateUrl: './orders.html',
  styleUrl: './orders.css',
})
export class Orders implements OnInit {
  private fb = inject(FormBuilder);
  private orderService = inject(OrderService);
  private router = inject(Router);
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

  filterForm = this.fb.group({
    from: [''],
    to: [''],
    status: [''],
    street: [''],
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

  goToShop(): void {
    this.router.navigate(['/products']);
  }

  loadOrders(): void {
    const { from, to, status, street } = this.filterForm.getRawValue();

    if (this.isDispatcher() && (!from || !to)) {
      this.errorMessage.set('As a dispatcher you must indicate the date range (from and to).');
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
        this.errorMessage.set(err.error?.message ?? 'Could not load orders.');
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
        this.errorMessage.set(err.error?.message ?? `Could not load order ${orderId}.`);
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
      this.errorMessage.set('Enter a valid order number (ID).');
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
        this.successMessage.set(`Order updated to "${this.statusLabel(res.status)}".`);

        if (this.showDetail() && this.detail()?.orderId === orderId) {
          this.openDetail(orderId);
        }
        if (!this.isAdmin()) {
          this.loadOrders();
        }
      },
      error: (err) => {
        this.statusUpdatingId.set(null);
        this.errorMessage.set(err.error?.message ?? 'Could not update order status.');
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

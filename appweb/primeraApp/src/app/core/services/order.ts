import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface OrderSummary {
  orderId: number;
  orderNumber: number;
  clientId: number;
  clientFullName: string;
  orderDate: string;
  status: string;
  totalCost: number;
  productCount: number;
}

export interface OrderProductDetail {
  code: string;
  name: string;
  price: number;
  category: string;
  promotionName: string | null;
  discountPercentage: number | null;
  quantity: number;
}

export interface OrderDetail {
  orderId: number;
  orderNumber: number;
  clientId: number;
  clientFullName: string;
  orderDate: string;
  status: string;
  subtotal: number;
  shippingCost: number;
  tax: number;
  totalCost: number;
  products: OrderProductDetail[];
}

export interface OrderProductRequest {
  productCode: string;
  quantity: number;
}

export interface CreateOrderRequest {
  clientId: number;
  deliveryType: string;
  street: string;
  doorNumber: string;
  apartment: string;
  products: OrderProductRequest[];
}

export interface CreateOrderResult {
  clientId: number;
  orderNumber: number;
  subtotal: number;
  shippingCost: number;
  tax: number;
  total: number;
}

export interface UpdateStatusResult {
  status: string;
  updatedAt: string;
}

export interface OrderFilters {
  from?: string | null;
  to?: string | null;
  status?: string | null;
  street?: string | null;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5128/api/orders';

  getAll(filters: OrderFilters = {}): Observable<OrderSummary[]> {
    let params = new HttpParams();
    if (filters.from) params = params.set('from', filters.from);
    if (filters.to) params = params.set('to', filters.to);
    if (filters.status?.trim()) params = params.set('status', filters.status.trim());
    if (filters.street?.trim()) params = params.set('street', filters.street.trim());

    return this.http.get<OrderSummary[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<OrderDetail> {
    return this.http.get<OrderDetail>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateOrderRequest): Observable<CreateOrderResult> {
    return this.http.post<CreateOrderResult>(this.apiUrl, request);
  }

  updateStatus(id: number, action: string): Observable<UpdateStatusResult> {
    return this.http.patch<UpdateStatusResult>(`${this.apiUrl}/${id}`, { action });
  }
}

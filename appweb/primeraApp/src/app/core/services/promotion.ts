import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductResponse } from './product';
import { PagedResult } from '../models/paged-result';
import { environment } from '../../../environments/environment';

export interface PromotionResponse {
  id: number;
  name: string;
  discountPercentage: number;
  dateFrom: string;
  dateTo: string;
  products: string[];
}

export interface PromotionRequest {
  name: string;
  discount: number;
  dateFrom: string;
  dateTo: string;
}

export interface PromotionFilters {
  date?: string | null;
  line?: string | null;
  product?: string | null;
  pageNumber?: number | null;
  pageSize?: number | null;
}

@Injectable({ providedIn: 'root' })
export class PromotionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/promotions`;

  getAll(filters: PromotionFilters = {}): Observable<PagedResult<PromotionResponse>> {
    let params = new HttpParams();
    if (filters.date?.trim()) params = params.set('date', filters.date.trim());
    if (filters.line?.trim()) params = params.set('line', filters.line.trim());
    if (filters.product?.trim()) params = params.set('product', filters.product.trim());
    if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber);
    if (filters.pageSize) params = params.set('pageSize', filters.pageSize);

    return this.http.get<PagedResult<PromotionResponse>>(this.apiUrl, { params });
  }

  create(data: PromotionRequest): Observable<PromotionResponse> {
    return this.http.post<PromotionResponse>(this.apiUrl, data);
  }

  update(id: number, data: PromotionRequest): Observable<PromotionResponse> {
    return this.http.put<PromotionResponse>(`${this.apiUrl}/${id}`, data);
  }

  addProduct(id: number, productCode: string): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(`${this.apiUrl}/${id}/products`, { productCode });
  }

  removeProduct(id: number, code: string): Observable<void> {
    const params = new HttpParams().set('code', code);
    return this.http.delete<void>(`${this.apiUrl}/${id}/products`, { params });
  }
}

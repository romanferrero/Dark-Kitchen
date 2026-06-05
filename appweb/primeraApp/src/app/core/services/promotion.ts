import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductResponse } from './product';

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
}

@Injectable({ providedIn: 'root' })
export class PromotionService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5128/api/promotions';

  getAll(filters: PromotionFilters = {}): Observable<PromotionResponse[]> {
    let params = new HttpParams();
    if (filters.date?.trim()) params = params.set('date', filters.date.trim());
    if (filters.line?.trim()) params = params.set('line', filters.line.trim());
    if (filters.product?.trim()) params = params.set('product', filters.product.trim());

    return this.http.get<PromotionResponse[]>(this.apiUrl, { params });
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

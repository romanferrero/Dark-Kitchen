import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/paged-result';
import { environment } from '../../../environments/environment';

export interface ProductResponse {
  id: number;
  code: string;
  name: string;
  price: number;
  description: string;
  line: string;
  category: string;
  active: boolean;
  imageUrls: string[];
}

export interface ProductRequest {
  name: string;
  price: number;
  description: string;
  line: string;
  category: string;
  images: string;
  active: boolean;
}

export interface ProductFilters {
  name?: string;
  line?: string;
  categories?: string;
  pageNumber?: number | null;
  pageSize?: number | null;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/products`;

  getAll(filters: ProductFilters = {}): Observable<PagedResult<ProductResponse>> {
    let params = new HttpParams();
    if (filters.name?.trim()) params = params.set('name', filters.name.trim());
    if (filters.line?.trim()) params = params.set('line', filters.line.trim());
    if (filters.categories?.trim()) params = params.set('categories', filters.categories.trim());
    if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber);
    if (filters.pageSize) params = params.set('pageSize', filters.pageSize);

    return this.http.get<PagedResult<ProductResponse>>(this.apiUrl, { params });
  }

  create(data: ProductRequest): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(this.apiUrl, data);
  }

  update(id: number, data: ProductRequest): Observable<ProductResponse> {
    return this.http.put<ProductResponse>(`${this.apiUrl}/${id}`, data);
  }
}

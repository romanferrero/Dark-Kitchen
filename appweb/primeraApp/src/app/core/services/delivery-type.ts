import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DeliveryTypeResponse {
  id: number;
  name: string;
  shippingCost: number;
}

export interface DeliveryTypeRequest {
  name: string;
  shippingCost: number;
}

@Injectable({ providedIn: 'root' })
export class DeliveryTypeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/delivery-types`;

  getAll(): Observable<DeliveryTypeResponse[]> {
    return this.http.get<DeliveryTypeResponse[]>(this.apiUrl);
  }

  create(data: DeliveryTypeRequest): Observable<DeliveryTypeResponse> {
    return this.http.post<DeliveryTypeResponse>(this.apiUrl, data);
  }

  update(id: number, data: DeliveryTypeRequest): Observable<DeliveryTypeResponse> {
    return this.http.put<DeliveryTypeResponse>(`${this.apiUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

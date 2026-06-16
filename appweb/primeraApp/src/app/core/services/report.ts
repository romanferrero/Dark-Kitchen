import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface TopProduct {
  code: string;
  name: string;
  quantitySold: number;
  imageUrls: string[];
}

export interface ClientSales {
  clientName: string;
  total: number;
}

export interface MonthlySales {
  period: string;
  clientSales: ClientSales[];
  monthlyTotal: number;
}

export interface SalesReport {
  monthlySales: MonthlySales[];
  grandTotal: number;
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/reports`;

  getSalesReport(): Observable<SalesReport> {
    const params = new HttpParams().set('type', 'sales');
    return this.http.get<SalesReport>(this.apiUrl, { params });
  }

  getTopProducts(dateFrom: string, dateTo: string): Observable<TopProduct[]> {
    const params = new HttpParams()
      .set('type', 'top-products')
      .set('dateFrom', dateFrom)
      .set('dateTo', dateTo);

    return this.http.get<TopProduct[]>(this.apiUrl, { params });
  }
}

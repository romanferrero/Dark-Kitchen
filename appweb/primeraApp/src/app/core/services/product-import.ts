import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ImporterParameter {
  name: string;
  label: string;
  description: string;
  required: boolean;
}

export interface ImporterInfo {
  name: string;
  description: string;
  parameters: ImporterParameter[];
}

export interface ImportRequest {
  importerName: string;
  parameters: Record<string, string>;
}

export interface ImportResult {
  importedCount: number;
  errors: string[];
}

@Injectable({ providedIn: 'root' })
export class ProductImport {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/products`;

  getImporters(): Observable<ImporterInfo[]> {
    return this.http.get<ImporterInfo[]>(`${this.apiUrl}/importers`);
  }

  importProducts(request: ImportRequest): Observable<ImportResult> {
    return this.http.post<ImportResult>(`${this.apiUrl}/import`, request);
  }
}

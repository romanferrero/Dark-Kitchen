import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AuditLogResponse {
  id: number;
  timestamp: string;
  entityName: string;
  entityId: number;
  description: string;
  responsibleUser: string;
}

export interface AuditLogFilters {
  dateFrom: string;
  dateTo: string;
  entityName?: string;
  entityId?: number;
}

@Injectable({ providedIn: 'root' })
export class AuditService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5128/api/audit';

  getAuditLogs(filters: AuditLogFilters): Observable<AuditLogResponse[]> {
    let params = new HttpParams()
      .set('dateFrom', filters.dateFrom)
      .set('dateTo', filters.dateTo);

    if (filters.entityName) {
      params = params.set('entityName', filters.entityName);
    }

    if (filters.entityId !== undefined && filters.entityId !== null) {
      params = params.set('entityId', filters.entityId.toString());
    }

    return this.http.get<AuditLogResponse[]>(this.apiUrl, { params });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateUserRequest {
 firstName: string;
 lastName: string;
 email: string;
 phone: string;
 password: string;
 role: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
 private http = inject(HttpClient);
 private apiUrl = 'http://localhost:5128/api/users';

 create(data: CreateUserRequest): Observable<void> {
 return this.http.post<void>(this.apiUrl, data);
 }
}

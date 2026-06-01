import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user?: { id: number; name: string };
}

@Injectable({ providedIn: 'root' })
export class Auth {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = 'http://localhost:5128/api/sessions';

  private readonly TOKEN_KEY = 'token';
  isAuthenticated = signal<boolean>(this.hasToken());

  login(credentials: LoginRequest): Observable<string> {
    return this.http.post(this.apiUrl, credentials, { responseType: 'text' }).pipe(
      tap((token) => {
        localStorage.setItem(this.TOKEN_KEY, token);
        this.isAuthenticated.set(true);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.isAuthenticated.set(false);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return this.hasToken();
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }
}

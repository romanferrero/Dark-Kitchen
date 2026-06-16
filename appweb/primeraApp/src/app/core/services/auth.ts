import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user?: { id: number; name: string };
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class Auth {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/api/sessions`;
  private clientsUrl = `${environment.apiUrl}/api/clients`;

  private readonly TOKEN_KEY = 'token';
  isAuthenticated = signal<boolean>(this.isTokenValid());
  role = signal<string | null>(this.readRole());
  permissions = signal<string[]>(this.readPermissions());

  register(data: RegisterRequest): Observable<void> {
    return this.http.post<void>(this.clientsUrl, data);
  }

  login(credentials: LoginRequest): Observable<string> {
    return this.http.post(this.apiUrl, credentials, { responseType: 'text' }).pipe(
      tap((token) => {
        localStorage.setItem(this.TOKEN_KEY, token);
        this.isAuthenticated.set(true);
        this.role.set(this.readRole());
        this.permissions.set(this.readPermissions());
      }),
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.isAuthenticated.set(false);
    this.role.set(null);
    this.permissions.set([]);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return this.isTokenValid();
  }

  getRole(): string | null {
    return this.readRole();
  }

  getPermissions(): string[] {
    return this.readPermissions();
  }

  hasPermission(permission: string): boolean {
    return this.permissions().includes(permission);
  }

  hasAnyPermission(permissions: string[]): boolean {
    return permissions.some((permission) => this.permissions().includes(permission));
  }

  getUserId(): number | null {
    const claims = this.decodeToken();
    if (!claims) return null;

    const raw = this.findClaim(claims, ['nameid', 'sub', 'nameidentifier']);
    const id = raw !== null ? Number(raw) : NaN;
    return Number.isFinite(id) ? id : null;
  }

  getEmail(): string | null {
    const claims = this.decodeToken();
    if (!claims) return null;
    return this.findClaim(claims, ['email', 'emailaddress']);
  }

  // devuelve el payload del token ya decodificado, para mostrarlo a modo informativo
  getClaims(): Record<string, unknown> | null {
    return this.decodeToken();
  }

  getExpiration(): Date | null {
    const claims = this.decodeToken();
    const exp = claims?.['exp'];
    return typeof exp === 'number' ? new Date(exp * 1000) : null;
  }

  private isTokenValid(): boolean {
    if (!localStorage.getItem(this.TOKEN_KEY)) return false;
    const expiration = this.getExpiration();
    return expiration !== null && expiration.getTime() > Date.now();
  }

  private readRole(): string | null {
    const claims = this.decodeToken();
    if (!claims) return null;
    return this.findClaim(claims, ['role']);
  }

  // el back manda un claim "permissions" con la lista de permisos del rol; puede venir
  // como array (varios permisos) o como string suelto (uno solo)
  private readPermissions(): string[] {
    const claims = this.decodeToken();
    if (!claims) return [];

    const raw = claims['permissions'];
    if (Array.isArray(raw)) {
      return raw.filter((p): p is string => typeof p === 'string');
    }
    return typeof raw === 'string' ? [raw] : [];
  }

  // este metodo extrae el payload del token, lo decodifica y lo parsea a un objeto
  private decodeToken(): Record<string, unknown> | null {
    const token = this.getToken();
    if (!token) return null;

    const payload = token.split('.')[1];
    if (!payload) return null;

    try {
      const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(json) as Record<string, unknown>;
    } catch {
      return null;
    }
  }

  // este metodo agarra los claims del token y busca alguno que coincida con los "names" que le pasamos por ejemplo: "role", "sub" o "nameid" dependiendo de como los tengamos en el back.
  private findClaim(claims: Record<string, unknown>, names: string[]): string | null {
    for (const name of names) {
      if (typeof claims[name] === 'string') {
        return claims[name] as string;
      }
      const match = Object.keys(claims).find(
        (key) => key === name || key.toLowerCase().endsWith('/' + name),
      );
      if (match && typeof claims[match] === 'string') {
        return claims[match] as string;
      }
    }
    return null;
  }
}

import { Component, computed, inject } from '@angular/core';
import { Auth } from '../../core/services/auth';
import { Badge } from '../../shared/components/badge/badge';

interface ClaimRow {
  label: string;
  value: string;
  raw: string;
}

const ROLE_LABELS: Record<string, string> = {
  Admin: 'Administrator',
  Dispatcher: 'Dispatcher',
  Client: 'Client',
};

// claims cuyo valor es un timestamp UNIX (segundos)
const TIME_CLAIMS = new Set(['exp', 'iat', 'nbf', 'auth_time']);

@Component({
  selector: 'app-profile',
  imports: [Badge],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  private auth = inject(Auth);

  email = computed(() => this.auth.getEmail() ?? '—');
  userId = computed(() => this.auth.getUserId());
  expiresAt = computed(() => this.auth.getExpiration());
  roleLabel = computed(() => {
    const role = this.auth.getRole();
    return role ? (ROLE_LABELS[role] ?? role) : 'No role';
  });
  initials = computed(() => {
    const email = this.auth.getEmail();
    if (!email) return 'U';
    const local = email.split('@')[0];
    const parts = local.split(/[._-]+/).filter(Boolean);
    const chars = parts.length >= 2 ? parts[0][0] + parts[1][0] : local.slice(0, 2);
    return chars.toUpperCase();
  });

  claims = computed<ClaimRow[]>(() => {
    const claims = this.auth.getClaims();
    if (!claims) return [];
    return Object.entries(claims).map(([key, value]) => ({
      label: this.friendlyLabel(key),
      value: this.formatValue(key, value),
      raw: key,
    }));
  });

  private friendlyLabel(key: string): string {
    const lower = key.toLowerCase();
    if (lower.endsWith('nameidentifier') || lower === 'nameid' || lower === 'sub') return 'User ID';
    if (lower.endsWith('emailaddress') || lower === 'email') return 'Email';
    if (lower.endsWith('/role') || lower === 'role') return 'Role';
    if (key === 'exp') return 'Expires';
    if (key === 'iat') return 'Issued';
    if (key === 'nbf') return 'Valid from';
    return key;
  }

  private formatValue(key: string, value: unknown): string {
    if (TIME_CLAIMS.has(key) && typeof value === 'number') {
      return new Date(value * 1000).toLocaleString();
    }
    return String(value);
  }
}

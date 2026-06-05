import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Auth } from '../../../core/services/auth';

const ROLE_LABELS: Record<string, string> = {
 Admin: 'Administrador',
 Dispatcher: 'Despachante',
 Client: 'Cliente',
};

@Component({
 selector: 'app-sidebar',
 imports: [RouterLink, RouterLinkActive],
 templateUrl: './sidebar.html',
 styleUrl: './sidebar.css',
 host: {
 class: 'block relative shrink-0 transition-[width] duration-200 ease-out',
 '[class.w-64]': 'locked()',
 '[class.w-16]': '!locked()',
 '(document:click)': 'closeMenu()',
 },
})
export class Sidebar {
 private auth = inject(Auth);
 private router = inject(Router);

 private readonly LOCK_KEY = 'sidebar-locked';

 locked = signal<boolean>(localStorage.getItem(this.LOCK_KEY) === 'true');
 hovered = signal(false);
 menuOpen = signal(false);

 expanded = computed(() => this.locked() || this.hovered());

 email = computed(() => this.auth.getEmail() ?? 'Usuario');
 roleLabel = computed(() => {
 const role = this.auth.getRole();
 return role ? (ROLE_LABELS[role] ?? role) : 'Sin rol';
 });
 initials = computed(() => {
 const email = this.auth.getEmail();
 if (!email) return 'U';
 const local = email.split('@')[0];
 const parts = local.split(/[._-]+/).filter(Boolean);
 const chars = parts.length >= 2 ? parts[0][0] + parts[1][0] : local.slice(0, 2);
 return chars.toUpperCase();
 });

 toggleLock(): void {
 const next = !this.locked();
 this.locked.set(next);
 localStorage.setItem(this.LOCK_KEY, String(next));
 }

 setHover(value: boolean): void {
 this.hovered.set(value);
 if (!value && !this.locked()) this.menuOpen.set(false);
 }

 toggleMenu(event: MouseEvent): void {
 event.stopPropagation();
 this.menuOpen.update(open => !open);
 }

 closeMenu(): void {
 this.menuOpen.set(false);
 }

 goProfile(): void {
 this.menuOpen.set(false);
 this.router.navigate(['/profile']);
 }

 logout(): void {
 this.menuOpen.set(false);
 this.auth.logout();
 }
}

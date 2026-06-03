import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest-guard';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: 'auth/login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login').then(m => m.Login),
  },
  {
    path: 'auth/register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/register/register').then(m => m.Register),
  },

  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },

  {
    path: '',
    loadComponent: () =>
      import('./layouts/main-layout/main-layout').then(m => m.MainLayout),
    children: [
      {
        path: 'home',
        loadComponent: () =>
          import('./features/home/home').then(m => m.Home),
      },
      {
        path: 'users',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/users/users').then(m => m.Users),
      },
    ],
  },

  { path: '**', redirectTo: 'auth/login' },
];

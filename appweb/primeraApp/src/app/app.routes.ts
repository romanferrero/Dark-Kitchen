import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest-guard';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: 'auth/login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'auth/register',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
  },

  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },

  {
    path: '',
    loadComponent: () => import('./layouts/main-layout/main-layout').then((m) => m.MainLayout),
    children: [
      {
        path: 'home',
        loadComponent: () => import('./features/home/home').then((m) => m.Home),
      },
      {
        path: 'products',
        canActivate: [authGuard],
        loadComponent: () => import('./features/products/products').then((m) => m.Products),
      },
      {
        path: 'products/import',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/products/product-import/product-import').then(
            (m) => m.ProductImportComponent,
          ),
      },
      {
        path: 'orders',
        canActivate: [authGuard],
        loadComponent: () => import('./features/orders/orders').then((m) => m.Orders),
      },
      {
        path: 'users',
        canActivate: [authGuard],
        loadComponent: () => import('./features/users/users').then((m) => m.Users),
      },
      {
        path: 'profile',
        canActivate: [authGuard],
        loadComponent: () => import('./features/profile/profile').then((m) => m.Profile),
      },
      {
        path: 'delivery-types',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/delivery-types/delivery-types').then((m) => m.DeliveryTypes),
      },
      {
        path: 'reports',
        canActivate: [authGuard],
        loadComponent: () => import('./features/reports/reports').then((m) => m.Reports),
      },
      {
        path: 'promotions',
        canActivate: [authGuard],
        loadComponent: () => import('./features/promotions/promotions').then((m) => m.Promotions),
      },
      {
        path: 'audit',
        canActivate: [authGuard],
        loadComponent: () => import('./features/audit/audit').then((m) => m.Audit),
      },
    ],
  },

  { path: '**', redirectTo: 'auth/login' },
];

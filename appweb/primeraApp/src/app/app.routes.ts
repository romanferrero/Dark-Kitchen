import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest-guard';
import { authGuard } from './core/guards/auth-guard';
import { permissionGuard } from './core/guards/permission-guard';

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
        canActivate: [permissionGuard],
        data: { permission: 'ViewProducts' },
        loadComponent: () => import('./features/products/products').then((m) => m.Products),
      },
      {
        path: 'products/import',
        canActivate: [permissionGuard],
        data: { permission: 'ManageProducts' },
        loadComponent: () =>
          import('./features/products/product-import/product-import').then(
            (m) => m.ProductImportComponent,
          ),
      },
      {
        path: 'orders',
        canActivate: [permissionGuard],
        data: { permission: ['ListOrders', 'ViewOrderDetail'] },
        loadComponent: () => import('./features/orders/orders').then((m) => m.Orders),
      },
      {
        path: 'users',
        canActivate: [permissionGuard],
        data: { permission: 'ManageInternalUsers' },
        loadComponent: () => import('./features/users/users').then((m) => m.Users),
      },
      {
        path: 'profile',
        canActivate: [authGuard],
        loadComponent: () => import('./features/profile/profile').then((m) => m.Profile),
      },
      {
        path: 'delivery-types',
        canActivate: [permissionGuard],
        data: { permission: 'ManageDeliveryTypes' },
        loadComponent: () =>
          import('./features/delivery-types/delivery-types').then((m) => m.DeliveryTypes),
      },
      {
        path: 'reports',
        canActivate: [permissionGuard],
        data: { permission: 'ViewReports' },
        loadComponent: () => import('./features/reports/reports').then((m) => m.Reports),
      },
      {
        path: 'promotions',
        canActivate: [permissionGuard],
        data: { permission: 'ViewPromotions' },
        loadComponent: () => import('./features/promotions/promotions').then((m) => m.Promotions),
      },
      {
        path: 'audit',
        canActivate: [permissionGuard],
        data: { permission: 'ViewAuditLog' },
        loadComponent: () => import('./features/audit/audit').then((m) => m.Audit),
      },
    ],
  },

  { path: '**', redirectTo: 'auth/login' },
];

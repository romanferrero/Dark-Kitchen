import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth/login',
    loadComponent: () =>
      import('./features/auth/login/login').then(m => m.Login),
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
        path: 'products/import',
        loadComponent: () =>
          import('./features/products/product-import/product-import').then(m => m.ProductImportComponent),
      },
    ],
  },

  { path: '**', redirectTo: 'auth/login' },
];

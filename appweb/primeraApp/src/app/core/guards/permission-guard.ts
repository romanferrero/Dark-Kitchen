import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Auth } from '../services/auth';

// bloquea la navegacion directa por URL: ademas de exigir estar logueado, valida que el
// rol tenga el permiso declarado en la data de la ruta (data: { permission: '...' })
export const permissionGuard: CanActivateFn = (route) => {
  const auth = inject(Auth);
  const router = inject(Router);

  if (!auth.isLoggedIn()) {
    router.navigate(['/auth/login']);
    return false;
  }

  const required = route.data?.['permission'] as string | string[] | undefined;
  if (!required) return true;

  const allowed = Array.isArray(required)
    ? auth.hasAnyPermission(required)
    : auth.hasPermission(required);
  if (allowed) return true;

  router.navigate(['/home']);
  return false;
};

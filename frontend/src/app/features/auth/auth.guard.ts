import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../trade/data-access/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const requiredRole = route.data?.['role'] as string | undefined;
  const user = authService.currentUser;

  if (user && (!requiredRole || user.role === requiredRole)) {
    return true;
  }

  const adminRoute = state.url.startsWith('/admin') || state.url.startsWith('/backoffice') || requiredRole === 'Admin';
  const loginPath = adminRoute ? '/admin/login' : '/login';
  return router.createUrlTree([loginPath], { queryParams: { returnUrl: state.url } });
};

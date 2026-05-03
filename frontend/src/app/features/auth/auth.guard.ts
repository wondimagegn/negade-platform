import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../trade/data-access/auth.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.currentUser) {
    return true;
  }

  const loginPath = state.url.startsWith('/admin') ? '/admin/login' : '/login';
  return router.createUrlTree([loginPath], { queryParams: { returnUrl: state.url } });
};

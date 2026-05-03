import { Routes } from '@angular/router';
import { authGuard } from './features/auth/auth.guard';
import { LoginPageComponent } from './features/auth/login-page.component';
import { AdminConsolePageComponent } from './features/admin/pages/admin-console-page.component';
import { BuyerPortalPageComponent } from './features/trade/pages/buyer-portal-page.component';
import { SupplierPortalPageComponent } from './features/trade/pages/supplier-portal-page.component';
import { TradeHubPageComponent } from './features/trade/pages/trade-hub-page.component';
import { BackofficeLayoutComponent } from './layouts/backoffice-layout.component';
import { PortalLayoutComponent } from './layouts/portal-layout.component';
import { ProductPageComponent } from './features/products/pages/product-page.component';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'portal/buyer', pathMatch: 'full' },
  { path: 'login', component: LoginPageComponent, data: { audience: 'trader' } },
  { path: 'admin/login', component: LoginPageComponent, data: { audience: 'admin' } },
  {
    path: 'backoffice',
    component: BackofficeLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', component: AdminConsolePageComponent },
      { path: 'products', component: ProductPageComponent }
    ]
  },
  {
    path: 'portal',
    component: PortalLayoutComponent,
    children: [
      { path: '', redirectTo: 'buyer', pathMatch: 'full' },
      { path: 'buyer', component: BuyerPortalPageComponent },
      { path: 'supplier', component: SupplierPortalPageComponent, canActivate: [authGuard] },
      { path: 'trade', component: TradeHubPageComponent }
    ]
  },
  { path: 'admin', redirectTo: 'backoffice', pathMatch: 'full' },
  { path: 'products-admin', redirectTo: 'backoffice/products', pathMatch: 'full' },
  { path: 'products', redirectTo: 'portal/buyer', pathMatch: 'full' },
  { path: '**', redirectTo: 'portal/buyer' }
];

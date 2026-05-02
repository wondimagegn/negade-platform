import { Routes } from '@angular/router';
import { TradeHubPageComponent } from './features/trade/pages/trade-hub-page.component';
import { ProductPageComponent } from './products/pages/product-page/product-page.component';

export const appRoutes: Routes = [
  { path: '', component: TradeHubPageComponent },
  { path: 'products-admin', component: ProductPageComponent },
  { path: 'products', redirectTo: '', pathMatch: 'full' },
  { path: '**', redirectTo: '' }
];

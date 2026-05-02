import { Routes } from '@angular/router';
import { ProductPageComponent } from './products/pages/product-page/product-page.component';

export const appRoutes: Routes = [
  { path: '', component: ProductPageComponent },
  { path: 'products', redirectTo: '', pathMatch: 'full' },
  { path: '**', redirectTo: '' }
];

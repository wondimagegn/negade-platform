import { Routes } from '@angular/router';
import { HomePageComponent } from './home-page.component';
import { ProductPageComponent } from './features/products/pages/product-page.component';

export const appRoutes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'products', component: ProductPageComponent },
  { path: '**', redirectTo: '' }
];

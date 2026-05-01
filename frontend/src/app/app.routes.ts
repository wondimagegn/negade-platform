import { Routes } from '@angular/router';
import { HomePageComponent } from './home-page.component';
import { ProductPageComponent } from './products/product-page.component';

export const appRoutes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'products', component: ProductPageComponent },
  { path: '**', redirectTo: '' }
];

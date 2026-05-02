import { Routes } from '@angular/router';
import { HomePageComponent } from './home/home-page.component';
import { ProductPageComponent } from './products/pages/product-page/product-page.component';

export const appRoutes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'products', component: ProductPageComponent },
  { path: '**', redirectTo: '' }
];

import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <main class="container">
      <h1>Negade</h1>

      <nav class="top-nav">
        <a routerLink="" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Trade Hub</a>
        <a routerLink="products-admin" routerLinkActive="active">Product Admin</a>
      </nav>

      <router-outlet />
    </main>
  `
})
export class AppComponent {}

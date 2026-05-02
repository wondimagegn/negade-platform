import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="card">
      <h2>Frontend Pages</h2>
      <p>Select a page below:</p>

      <nav class="page-links">
        <a routerLink="/products">Product Management</a>
      </nav>
    </section>
  `
})
export class HomePageComponent {}

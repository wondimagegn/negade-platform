import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: `
    <main class="container">
      <h1>Product Management</h1>
      <router-outlet />
    </main>
  `
})
export class AppComponent {}

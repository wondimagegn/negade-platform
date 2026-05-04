import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../features/trade/data-access/auth.service';

@Component({
  selector: 'app-portal-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <header class="portal-header">
      <a class="logo" routerLink="/portal/buyer">
        <span class="logo-mark">N</span>
        <span>Negade</span>
      </a>

      <nav class="portal-nav">
        <a routerLink="/portal/buyer" routerLinkActive="active">Buyer</a>
        <a routerLink="/portal/supplier" routerLinkActive="active">Supplier</a>
        <a routerLink="/portal/trade" routerLinkActive="active">Trade Hub</a>
      </nav>

      <div class="portal-actions" *ngIf="authService.currentUser$ | async as user; else portalGuest">
        <span class="portal-user">{{ user.fullName }}</span>
        <button type="button" (click)="logout()">Logout</button>
      </div>
      <ng-template #portalGuest>
        <a class="portal-login" routerLink="/login">Buyer/Supplier Login</a>
      </ng-template>
    </header>

    <main class="portal-main">
      <router-outlet />
    </main>
  `
})
export class PortalLayoutComponent {
  readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../features/trade/data-access/auth.service';

@Component({
  selector: 'app-backoffice-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <header class="app-header backoffice-header">
      <div class="header-left">
        <a class="logo" routerLink="/backoffice">
          <span class="logo-mark">N</span>
          <span>Negade Backoffice</span>
        </a>
        <nav class="top-nav">
          <a routerLink="/backoffice" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Operations</a>
          <a routerLink="/portal/buyer">Buyer Portal</a>
          <a routerLink="/portal/supplier">Supplier Portal</a>
        </nav>
      </div>

      <div class="header-actions" *ngIf="authService.currentUser$ | async as user">
        <button type="button" class="header-icon" aria-label="Notifications">
          <span class="material-symbol">notifications</span>
          <span class="notification-dot">3</span>
        </button>
        <div class="user-chip">
          <span class="avatar">{{ user.fullName.charAt(0) }}</span>
          <span>
            <strong>{{ user.fullName }}</strong>
            <small>{{ user.role }}</small>
          </span>
        </div>
        <button type="button" class="logout-button" (click)="logout()">Logout</button>
      </div>
    </header>

    <router-outlet />
  `
})
export class BackofficeLayoutComponent {
  readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }
}

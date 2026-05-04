import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../trade/data-access/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  mode: 'login' | 'register' = 'login';
  audience: 'admin' | 'trader' = 'trader';
  authenticating = false;
  errorMessage = '';

  readonly loginForm = this.fb.nonNullable.group({
    identifier: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  readonly registerForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(200)]],
    phoneNumber: ['', [Validators.required]],
    userName: [''],
    email: [''],
    password: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.audience = this.route.snapshot.data['audience'] === 'admin' ? 'admin' : 'trader';
    this.mode = this.audience === 'admin' ? 'login' : this.mode;

    const currentUser = this.authService.currentUser;
    if (currentUser && (this.audience !== 'admin' || currentUser.role === 'Admin')) {
      this.router.navigateByUrl(this.returnUrl);
      return;
    }

    if (currentUser && this.audience === 'admin') {
      this.authService.logout();
    }
  }

  get title(): string {
    return this.audience === 'admin' ? 'Backoffice sign in' : 'Buyer and supplier sign in';
  }

  get returnUrl(): string {
    return this.route.snapshot.queryParamMap.get('returnUrl') || (this.audience === 'admin' ? '/backoffice' : '/portal/buyer');
  }

  login(): void {
    if (this.loginForm.invalid || this.authenticating) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.authenticating = true;
    this.errorMessage = '';
    this.authService
      .login(this.loginForm.getRawValue())
      .pipe(finalize(() => (this.authenticating = false)))
      .subscribe({
        next: () => this.router.navigateByUrl(this.returnUrl),
        error: () => (this.errorMessage = 'Could not sign in with that email, username, phone, and password.')
      });
  }

  register(): void {
    if (this.registerForm.invalid || this.authenticating) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.authenticating = true;
    this.errorMessage = '';
    this.authService
      .register(this.registerForm.getRawValue())
      .pipe(finalize(() => (this.authenticating = false)))
      .subscribe({
        next: () => this.router.navigateByUrl(this.returnUrl),
        error: () => (this.errorMessage = 'Could not create this account. The phone, username, or email may already be registered.')
      });
  }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface AuthResponse {
  token: string;
  userId: string;
  fullName: string;
  phoneNumber: string;
  role: string;
}

export interface LoginPayload {
  phoneNumber: string;
  password: string;
}

export interface RegisterPayload extends LoginPayload {
  fullName: string;
  email?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'negade.auth';
  private readonly apiBaseUrl = environment.apiBaseUrl;
  private readonly authState = new BehaviorSubject<AuthResponse | null>(this.readStoredAuth());

  readonly currentUser$ = this.authState.asObservable();

  constructor(private readonly http: HttpClient) {}

  get currentUser(): AuthResponse | null {
    return this.authState.value;
  }

  get token(): string | null {
    return this.currentUser?.token ?? null;
  }

  register(payload: RegisterPayload): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiBaseUrl}/api/auth/register`, payload)
      .pipe(tap((response) => this.storeAuth(response)));
  }

  login(payload: LoginPayload): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiBaseUrl}/api/auth/login`, payload)
      .pipe(tap((response) => this.storeAuth(response)));
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.authState.next(null);
  }

  private storeAuth(response: AuthResponse): void {
    localStorage.setItem(this.storageKey, JSON.stringify(response));
    this.authState.next(response);
  }

  private readStoredAuth(): AuthResponse | null {
    const stored = localStorage.getItem(this.storageKey);
    return stored ? (JSON.parse(stored) as AuthResponse) : null;
  }
}

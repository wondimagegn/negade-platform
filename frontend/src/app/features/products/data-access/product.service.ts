import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, ProductPayload } from './product.models';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../trade/data-access/auth.service';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/product`;

  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService
  ) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  create(payload: ProductPayload): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, payload, { headers: this.authHeaders() });
  }

  update(id: string, payload: ProductPayload): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, payload, { headers: this.authHeaders() });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`, { headers: this.authHeaders() });
  }

  private authHeaders(): HttpHeaders {
    const token = this.authService.token;
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }
}

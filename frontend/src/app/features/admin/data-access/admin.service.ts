import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../trade/data-access/auth.service';
import { Rfq } from '../../trade/data-access/trade.models';
import { Category, CategoryPayload, City, CityPayload, Region, RegionPayload } from './admin.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/admin`;
  private readonly rfqUrl = `${environment.apiBaseUrl}/api/rfqs`;

  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService
  ) {}

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/categories`, { headers: this.authHeaders() });
  }

  createCategory(payload: CategoryPayload): Observable<Category> {
    return this.http.post<Category>(`${this.baseUrl}/categories`, payload, { headers: this.authHeaders() });
  }

  updateCategory(id: string, payload: CategoryPayload): Observable<Category> {
    return this.http.put<Category>(`${this.baseUrl}/categories/${id}`, payload, { headers: this.authHeaders() });
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/categories/${id}`, { headers: this.authHeaders() });
  }

  getRegions(): Observable<Region[]> {
    return this.http.get<Region[]>(`${this.baseUrl}/regions`, { headers: this.authHeaders() });
  }

  createRegion(payload: RegionPayload): Observable<Region> {
    return this.http.post<Region>(`${this.baseUrl}/regions`, payload, { headers: this.authHeaders() });
  }

  updateRegion(id: string, payload: RegionPayload): Observable<Region> {
    return this.http.put<Region>(`${this.baseUrl}/regions/${id}`, payload, { headers: this.authHeaders() });
  }

  deleteRegion(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/regions/${id}`, { headers: this.authHeaders() });
  }

  createCity(payload: CityPayload): Observable<City> {
    return this.http.post<City>(`${this.baseUrl}/cities`, payload, { headers: this.authHeaders() });
  }

  updateCity(id: string, payload: CityPayload): Observable<City> {
    return this.http.put<City>(`${this.baseUrl}/cities/${id}`, payload, { headers: this.authHeaders() });
  }

  deleteCity(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/cities/${id}`, { headers: this.authHeaders() });
  }

  updateRfqStatus(id: string, status: string): Observable<Rfq> {
    return this.http.patch<Rfq>(`${this.rfqUrl}/${id}/status`, { status }, { headers: this.authHeaders() });
  }

  private authHeaders(): HttpHeaders {
    const token = this.authService.token;
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }
}

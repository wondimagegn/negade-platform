import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  BusinessProfile,
  BusinessProfilePayload,
  Product,
  ProductSearchParams,
  Rfq,
  RfqPayload
} from './trade.models';

@Injectable({ providedIn: 'root' })
export class TradeService {
  private readonly apiBaseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  searchProducts(params: ProductSearchParams): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiBaseUrl}/api/marketplace/products`, {
      params: this.toParams(params)
    });
  }

  getBusinessProfiles(): Observable<BusinessProfile[]> {
    return this.http.get<BusinessProfile[]>(`${this.apiBaseUrl}/api/business-profiles`);
  }

  createBusinessProfile(payload: BusinessProfilePayload): Observable<BusinessProfile> {
    return this.http.post<BusinessProfile>(`${this.apiBaseUrl}/api/business-profiles`, payload);
  }

  getRfqs(): Observable<Rfq[]> {
    return this.http.get<Rfq[]>(`${this.apiBaseUrl}/api/rfqs`);
  }

  createRfq(payload: RfqPayload): Observable<Rfq> {
    return this.http.post<Rfq>(`${this.apiBaseUrl}/api/rfqs`, payload);
  }

  private toParams(params: ProductSearchParams): HttpParams {
    let httpParams = new HttpParams();

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && `${value}`.trim() !== '') {
        httpParams = httpParams.set(key, `${value}`.trim());
      }
    });

    return httpParams;
  }
}

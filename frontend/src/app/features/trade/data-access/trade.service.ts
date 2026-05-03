import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  BusinessProfile,
  BusinessProfilePayload,
  Product,
  ProductSearchParams,
  Quote,
  QuotePayload,
  Rfq,
  RfqPayload,
  SupplierQuote,
  TradeHistory,
  TradeHistoryPayload,
  TradeRating,
  TradeRatingPayload,
  UpdateBusinessProfilePayload
} from './trade.models';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class TradeService {
  private readonly apiBaseUrl = environment.apiBaseUrl;

  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService
  ) {}

  searchProducts(params: ProductSearchParams): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiBaseUrl}/api/marketplace/products`, {
      params: this.toParams(params)
    });
  }

  getBusinessProfiles(): Observable<BusinessProfile[]> {
    return this.http.get<BusinessProfile[]>(`${this.apiBaseUrl}/api/business-profiles`);
  }

  getMyBusinessProfiles(): Observable<BusinessProfile[]> {
    return this.http.get<BusinessProfile[]>(`${this.apiBaseUrl}/api/business-profiles/me`, {
      headers: this.authHeaders()
    });
  }

  createBusinessProfile(payload: BusinessProfilePayload): Observable<BusinessProfile> {
    return this.http.post<BusinessProfile>(`${this.apiBaseUrl}/api/business-profiles`, payload, {
      headers: this.authHeaders()
    });
  }

  verifyBusinessProfile(profileId: string, verificationStatus: string): Observable<BusinessProfile> {
    return this.http.patch<BusinessProfile>(
      `${this.apiBaseUrl}/api/business-profiles/${profileId}/verification`,
      { verificationStatus },
      { headers: this.authHeaders() }
    );
  }

  updateBusinessProfile(profileId: string, payload: UpdateBusinessProfilePayload): Observable<BusinessProfile> {
    return this.http.put<BusinessProfile>(`${this.apiBaseUrl}/api/business-profiles/${profileId}`, payload, {
      headers: this.authHeaders()
    });
  }

  getRfqs(): Observable<Rfq[]> {
    return this.http.get<Rfq[]>(`${this.apiBaseUrl}/api/rfqs`);
  }

  createRfq(payload: RfqPayload): Observable<Rfq> {
    return this.http.post<Rfq>(`${this.apiBaseUrl}/api/rfqs`, payload, {
      headers: this.authHeaders()
    });
  }

  getQuotes(rfqId: string): Observable<Quote[]> {
    return this.http.get<Quote[]>(`${this.apiBaseUrl}/api/rfqs/${rfqId}/quotes`);
  }

  getMyQuotes(): Observable<SupplierQuote[]> {
    return this.http.get<SupplierQuote[]>(`${this.apiBaseUrl}/api/rfqs/my-quotes`, {
      headers: this.authHeaders()
    });
  }

  createQuote(rfqId: string, payload: QuotePayload): Observable<Quote> {
    return this.http.post<Quote>(`${this.apiBaseUrl}/api/rfqs/${rfqId}/quotes`, payload, {
      headers: this.authHeaders()
    });
  }

  createRating(profileId: string, payload: TradeRatingPayload): Observable<TradeRating> {
    return this.http.post<TradeRating>(
      `${this.apiBaseUrl}/api/business-profiles/${profileId}/ratings`,
      payload,
      { headers: this.authHeaders() }
    );
  }

  createTradeHistory(profileId: string, payload: TradeHistoryPayload): Observable<TradeHistory> {
    return this.http.post<TradeHistory>(
      `${this.apiBaseUrl}/api/business-profiles/${profileId}/trade-history`,
      payload,
      { headers: this.authHeaders() }
    );
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

  private authHeaders(): HttpHeaders {
    const token = this.authService.token;
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }
}

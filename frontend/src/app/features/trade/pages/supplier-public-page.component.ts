import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../data-access/auth.service';
import { BusinessProfile, Rfq } from '../data-access/trade.models';
import { TradeService } from '../data-access/trade.service';

@Component({
  selector: 'app-supplier-public-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './supplier-public-page.component.html',
  styleUrl: './portal-pages.css'
})
export class SupplierPublicPageComponent implements OnInit {
  private readonly tradeService = inject(TradeService);
  readonly authService = inject(AuthService);

  rfqs: Rfq[] = [];
  suppliers: BusinessProfile[] = [];
  loading = false;
  errorMessage = '';

  get openRfqs(): Rfq[] {
    return this.rfqs.filter((rfq) => rfq.status === 'Open' || rfq.status === 'Quoted');
  }

  get verifiedSuppliers(): BusinessProfile[] {
    return this.suppliers.filter((supplier) => supplier.verificationStatus === 'Verified');
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      suppliers: this.tradeService.getBusinessProfiles(),
      rfqs: this.tradeService.getRfqs()
    }).subscribe({
      next: ({ suppliers, rfqs }) => {
        this.suppliers = suppliers;
        this.rfqs = rfqs;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load supplier market signals.';
        this.loading = false;
      }
    });
  }
}

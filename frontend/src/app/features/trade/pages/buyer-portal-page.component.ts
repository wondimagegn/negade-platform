import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { Product, Rfq } from '../data-access/trade.models';
import { TradeService } from '../data-access/trade.service';

@Component({
  selector: 'app-buyer-portal-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './buyer-portal-page.component.html',
  styleUrl: './portal-pages.css'
})
export class BuyerPortalPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly tradeService = inject(TradeService);

  products: Product[] = [];
  rfqs: Rfq[] = [];
  loading = false;
  saving = false;
  errorMessage = '';
  successMessage = '';

  readonly searchForm = this.fb.nonNullable.group({
    search: [''],
    category: [''],
    region: ['']
  });

  readonly rfqForm = this.fb.nonNullable.group({
    buyerName: ['', [Validators.required, Validators.maxLength(200)]],
    buyerPhoneNumber: ['', [Validators.required, Validators.maxLength(50)]],
    buyerBusinessName: ['', [Validators.maxLength(200)]],
    productName: ['', [Validators.required, Validators.maxLength(200)]],
    category: ['', [Validators.required, Validators.maxLength(100)]],
    quantity: [1, [Validators.required, Validators.min(0.01)]],
    unit: ['quintal', [Validators.required, Validators.maxLength(50)]],
    deliveryRegion: ['Addis Ababa', [Validators.required, Validators.maxLength(100)]],
    deliveryCity: ['Addis Ababa', [Validators.required, Validators.maxLength(100)]],
    notes: ['', [Validators.maxLength(2000)]]
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    forkJoin({
      products: this.tradeService.searchProducts(this.searchForm.getRawValue()),
      rfqs: this.tradeService.getRfqs()
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: ({ products, rfqs }) => {
          this.products = products;
          this.rfqs = rfqs;
        },
        error: () => (this.errorMessage = 'Could not load buyer workspace.')
      });
  }

  search(): void {
    this.loading = true;
    this.tradeService
      .searchProducts(this.searchForm.getRawValue())
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (products) => (this.products = products),
        error: () => (this.errorMessage = 'Could not search marketplace.')
      });
  }

  createRfq(): void {
    if (this.rfqForm.invalid || this.saving) {
      this.rfqForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.tradeService
      .createRfq(this.rfqForm.getRawValue())
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (rfq) => {
          this.rfqs = [rfq, ...this.rfqs];
          this.successMessage = 'RFQ submitted.';
        },
        error: () => (this.errorMessage = 'Could not submit RFQ. Login may be required.')
      });
  }
}

import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { BusinessProfile, Product, Rfq } from '../data-access/trade.models';
import { TradeService } from '../data-access/trade.service';

@Component({
  selector: 'app-trade-hub-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './trade-hub-page.component.html',
  styleUrl: './trade-hub-page.component.css'
})
export class TradeHubPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly tradeService = inject(TradeService);

  products: Product[] = [];
  suppliers: BusinessProfile[] = [];
  rfqs: Rfq[] = [];
  loading = false;
  savingRfq = false;
  savingSupplier = false;
  errorMessage = '';

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

  readonly supplierForm = this.fb.nonNullable.group({
    businessName: ['', [Validators.required, Validators.maxLength(200)]],
    ownerName: ['', [Validators.required, Validators.maxLength(200)]],
    tinNumber: ['', [Validators.maxLength(50)]],
    phoneNumber: ['', [Validators.required, Validators.maxLength(50)]],
    region: ['Addis Ababa', [Validators.required, Validators.maxLength(100)]],
    city: ['Addis Ababa', [Validators.required, Validators.maxLength(100)]],
    address: ['', [Validators.maxLength(500)]],
    businessType: ['Supplier', [Validators.required, Validators.maxLength(100)]]
  });

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      products: this.tradeService.searchProducts(this.searchForm.getRawValue()),
      suppliers: this.tradeService.getBusinessProfiles(),
      rfqs: this.tradeService.getRfqs()
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: ({ products, suppliers, rfqs }) => {
          this.products = products;
          this.suppliers = suppliers;
          this.rfqs = rfqs;
        },
        error: () => (this.errorMessage = 'Could not load trade data.')
      });
  }

  searchProducts(): void {
    this.loading = true;
    this.errorMessage = '';

    this.tradeService
      .searchProducts(this.searchForm.getRawValue())
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (products) => (this.products = products),
        error: () => (this.errorMessage = 'Could not search products.')
      });
  }

  createRfq(): void {
    if (this.rfqForm.invalid || this.savingRfq) {
      this.rfqForm.markAllAsTouched();
      return;
    }

    this.savingRfq = true;
    this.tradeService
      .createRfq(this.rfqForm.getRawValue())
      .pipe(finalize(() => (this.savingRfq = false)))
      .subscribe({
        next: (rfq) => {
          this.rfqs = [rfq, ...this.rfqs];
          this.rfqForm.reset({
            buyerName: '',
            buyerPhoneNumber: '',
            buyerBusinessName: '',
            productName: '',
            category: '',
            quantity: 1,
            unit: 'quintal',
            deliveryRegion: 'Addis Ababa',
            deliveryCity: 'Addis Ababa',
            notes: ''
          });
        },
        error: () => (this.errorMessage = 'Could not submit the RFQ.')
      });
  }

  createSupplier(): void {
    if (this.supplierForm.invalid || this.savingSupplier) {
      this.supplierForm.markAllAsTouched();
      return;
    }

    this.savingSupplier = true;
    this.tradeService
      .createBusinessProfile(this.supplierForm.getRawValue())
      .pipe(finalize(() => (this.savingSupplier = false)))
      .subscribe({
        next: (supplier) => {
          this.suppliers = [supplier, ...this.suppliers];
          this.supplierForm.reset({
            businessName: '',
            ownerName: '',
            tinNumber: '',
            phoneNumber: '',
            region: 'Addis Ababa',
            city: 'Addis Ababa',
            address: '',
            businessType: 'Supplier'
          });
        },
        error: () => (this.errorMessage = 'Could not create supplier profile.')
      });
  }
}

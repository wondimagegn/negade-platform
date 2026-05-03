import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { AuthResponse, AuthService } from '../data-access/auth.service';
import { BusinessProfile, Product, Quote, Rfq } from '../data-access/trade.models';
import { TradeService } from '../data-access/trade.service';

@Component({
  selector: 'app-trade-hub-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './trade-hub-page.component.html',
  styleUrl: './trade-hub-page.component.css'
})
export class TradeHubPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly tradeService = inject(TradeService);
  private readonly authService = inject(AuthService);

  products: Product[] = [];
  suppliers: BusinessProfile[] = [];
  rfqs: Rfq[] = [];
  quotes: Quote[] = [];
  currentUser: AuthResponse | null = null;
  loading = false;
  authenticating = false;
  savingRfq = false;
  savingSupplier = false;
  savingQuote = false;
  savingTrust = false;
  errorMessage = '';
  successMessage = '';

  readonly loginForm = this.fb.nonNullable.group({
    phoneNumber: [''],
    password: ['']
  });

  readonly registerForm = this.fb.nonNullable.group({
    fullName: [''],
    phoneNumber: [''],
    email: [''],
    password: ['']
  });

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

  readonly quoteForm = this.fb.nonNullable.group({
    rfqId: [''],
    supplierId: [''],
    unitPrice: [0, [Validators.required, Validators.min(0.01)]],
    quantityAvailable: [1, [Validators.required, Validators.min(0.01)]],
    deliveryTimeInDays: [1, [Validators.required, Validators.min(0)]],
    notes: ['']
  });

  readonly verificationForm = this.fb.nonNullable.group({
    supplierId: [''],
    verificationStatus: ['Verified']
  });

  readonly ratingForm = this.fb.nonNullable.group({
    supplierId: [''],
    score: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
    comment: ['']
  });

  readonly historyForm = this.fb.nonNullable.group({
    supplierId: [''],
    description: [''],
    counterpartyName: [''],
    amount: [0],
    tradeDate: [new Date().toISOString().slice(0, 10)]
  });

  get verifiedSuppliers(): BusinessProfile[] {
    return this.suppliers.filter((supplier) => supplier.verificationStatus === 'Verified');
  }

  get categoryOptions(): string[] {
    return [...new Set([
      ...this.products.map((product) => product.category),
      ...this.rfqs.map((rfq) => rfq.category)
    ].filter(Boolean))]
      .sort((first, second) => first.localeCompare(second));
  }

  get regionOptions(): string[] {
    return [...new Set([
      ...this.products.map((product) => product.region),
      ...this.suppliers.map((supplier) => supplier.region),
      ...this.rfqs.map((rfq) => rfq.deliveryRegion)
    ].filter(Boolean))]
      .sort((first, second) => first.localeCompare(second));
  }

  ngOnInit(): void {
    this.currentUser = this.authService.currentUser;
    this.authService.currentUser$.subscribe((user) => (this.currentUser = user));
    this.loadDashboard();
  }

  register(): void {
    if (this.authenticating) {
      return;
    }

    this.authenticating = true;
    this.errorMessage = '';
    this.authService
      .register(this.registerForm.getRawValue())
      .pipe(finalize(() => (this.authenticating = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Account created.';
          this.registerForm.reset({ fullName: '', phoneNumber: '', email: '', password: '' });
        },
        error: () => (this.errorMessage = 'Could not register this account.')
      });
  }

  login(): void {
    if (this.authenticating) {
      return;
    }

    this.authenticating = true;
    this.errorMessage = '';
    this.authService
      .login(this.loginForm.getRawValue())
      .pipe(finalize(() => (this.authenticating = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Signed in.';
          this.loginForm.reset({ phoneNumber: '', password: '' });
        },
        error: () => (this.errorMessage = 'Invalid phone number or password.')
      });
  }

  logout(): void {
    this.authService.logout();
    this.successMessage = 'Signed out.';
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
          this.setDefaultSelections();
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
          this.quoteForm.patchValue({ rfqId: rfq.id });
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
          this.setDefaultSelections();
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

  loadQuotes(rfqId: string): void {
    if (!rfqId) {
      this.quotes = [];
      return;
    }

    this.tradeService.getQuotes(rfqId).subscribe({
      next: (quotes) => (this.quotes = quotes),
      error: () => (this.errorMessage = 'Could not load quotes for this RFQ.')
    });
  }

  createQuote(): void {
    if (this.quoteForm.invalid || this.savingQuote) {
      this.quoteForm.markAllAsTouched();
      return;
    }

    const { rfqId, ...payload } = this.quoteForm.getRawValue();
    if (!rfqId || !payload.supplierId) {
      this.errorMessage = 'Choose an RFQ and supplier before submitting a quote.';
      return;
    }

    this.savingQuote = true;
    this.tradeService
      .createQuote(rfqId, payload)
      .pipe(finalize(() => (this.savingQuote = false)))
      .subscribe({
        next: (quote) => {
          this.quotes = [quote, ...this.quotes];
          this.loadDashboard();
        },
        error: () => (this.errorMessage = 'Could not submit the quote. Make sure you own the supplier profile.')
      });
  }

  verifySupplier(): void {
    const { supplierId, verificationStatus } = this.verificationForm.getRawValue();
    if (!supplierId || this.savingTrust) {
      this.errorMessage = 'Choose a supplier to verify.';
      return;
    }

    this.savingTrust = true;
    this.tradeService
      .verifyBusinessProfile(supplierId, verificationStatus)
      .pipe(finalize(() => (this.savingTrust = false)))
      .subscribe({
        next: (supplier) => {
          this.suppliers = this.suppliers.map((item) => (item.id === supplier.id ? supplier : item));
          this.successMessage = 'Verification updated.';
        },
        error: () => (this.errorMessage = 'Could not update verification status.')
      });
  }

  createRating(): void {
    const { supplierId, ...payload } = this.ratingForm.getRawValue();
    if (!supplierId || this.ratingForm.invalid || this.savingTrust) {
      this.ratingForm.markAllAsTouched();
      return;
    }

    this.savingTrust = true;
    this.tradeService
      .createRating(supplierId, payload)
      .pipe(finalize(() => (this.savingTrust = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Rating added.';
          this.loadDashboard();
        },
        error: () => (this.errorMessage = 'Could not add rating.')
      });
  }

  createTradeHistory(): void {
    const { supplierId, ...rawPayload } = this.historyForm.getRawValue();
    if (!supplierId || !rawPayload.description || this.savingTrust) {
      this.historyForm.markAllAsTouched();
      return;
    }

    this.savingTrust = true;
    this.tradeService
      .createTradeHistory(supplierId, {
        ...rawPayload,
        amount: rawPayload.amount || null,
        tradeDate: new Date(rawPayload.tradeDate).toISOString()
      })
      .pipe(finalize(() => (this.savingTrust = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Trade history added.';
          this.loadDashboard();
        },
        error: () => (this.errorMessage = 'Could not add trade history. Make sure you own the supplier profile.')
      });
  }

  private setDefaultSelections(): void {
    const firstSupplierId = this.suppliers[0]?.id ?? '';
    const firstRfqId = this.rfqs[0]?.id ?? '';
    this.quoteForm.patchValue({
      supplierId: this.quoteForm.value.supplierId || firstSupplierId,
      rfqId: this.quoteForm.value.rfqId || firstRfqId
    });
    this.verificationForm.patchValue({ supplierId: this.verificationForm.value.supplierId || firstSupplierId });
    this.ratingForm.patchValue({ supplierId: this.ratingForm.value.supplierId || firstSupplierId });
    this.historyForm.patchValue({ supplierId: this.historyForm.value.supplierId || firstSupplierId });

    if (firstRfqId && this.quotes.length === 0) {
      this.loadQuotes(firstRfqId);
    }
  }
}

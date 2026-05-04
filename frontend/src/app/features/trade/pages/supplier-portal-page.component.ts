import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { Product, ProductPayload } from '../../products/data-access/product.models';
import { ProductService } from '../../products/data-access/product.service';
import { BusinessProfile, Rfq, SupplierQuote } from '../data-access/trade.models';
import { TradeService } from '../data-access/trade.service';

@Component({
  selector: 'app-supplier-portal-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './supplier-portal-page.component.html',
  styleUrl: './portal-pages.css'
})
export class SupplierPortalPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly tradeService = inject(TradeService);
  private readonly productService = inject(ProductService);

  profiles: BusinessProfile[] = [];
  products: Product[] = [];
  rfqs: Rfq[] = [];
  quotes: SupplierQuote[] = [];
  loading = false;
  savingProfile = false;
  savingQuote = false;
  savingProduct = false;
  errorMessage = '';
  successMessage = '';
  editingProductId: string | null = null;

  readonly profileForm = this.fb.nonNullable.group({
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

  readonly productForm = this.fb.nonNullable.group({
    supplierId: [''],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.maxLength(2000)]],
    category: ['', [Validators.required, Validators.maxLength(100)]],
    price: [0, [Validators.required, Validators.min(0)]],
    unit: ['pcs', [Validators.required, Validators.maxLength(50)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
    availableQuantity: [0, [Validators.required, Validators.min(0)]],
    region: [''],
    city: [''],
    isAvailable: [true]
  });

  get verifiedProfiles(): BusinessProfile[] {
    return this.profiles.filter((profile) => profile.verificationStatus === 'Verified');
  }

  get selectedProfileIds(): string[] {
    return this.profiles.map((profile) => profile.id);
  }

  get ownedProducts(): Product[] {
    return this.products.filter((product) => product.supplierId && this.selectedProfileIds.includes(product.supplierId));
  }

  get openRfqs(): Rfq[] {
    return this.rfqs.filter((rfq) => rfq.status === 'Open' || rfq.status === 'Quoted');
  }

  get categoryOptions(): string[] {
    return [...new Set([
      ...this.rfqs.map((rfq) => rfq.category),
      ...this.products.map((product) => product.category)
    ].filter(Boolean))]
      .sort((first, second) => first.localeCompare(second));
  }

  get readinessScore(): number {
    let score = 0;
    if (this.profiles.length > 0) score += 30;
    if (this.verifiedProfiles.length > 0) score += 30;
    if (this.ownedProducts.length > 0) score += 25;
    if (this.profiles.some((profile) => profile.ratingCount > 0 || profile.tradeCount > 0)) score += 15;
    return score;
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    forkJoin({
      profiles: this.tradeService.getMyBusinessProfiles(),
      products: this.productService.getAll(),
      rfqs: this.tradeService.getRfqs(),
      quotes: this.tradeService.getMyQuotes()
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: ({ profiles, products, rfqs, quotes }) => {
          this.profiles = profiles;
          this.products = products;
          this.rfqs = rfqs;
          this.quotes = quotes;
          this.quoteForm.patchValue({
            supplierId: this.quoteForm.value.supplierId || profiles[0]?.id || '',
            rfqId: this.quoteForm.value.rfqId || rfqs[0]?.id || ''
          });
          this.productForm.patchValue({
            supplierId: this.productForm.value.supplierId || profiles[0]?.id || '',
            category: this.productForm.value.category || rfqs[0]?.category || '',
            region: this.productForm.value.region || profiles[0]?.region || '',
            city: this.productForm.value.city || profiles[0]?.city || ''
          });
        },
        error: () => (this.errorMessage = 'Could not load supplier workspace. Please login as a supplier first.')
      });
  }

  createProfile(): void {
    if (this.profileForm.invalid || this.savingProfile) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.savingProfile = true;
    this.tradeService
      .createBusinessProfile(this.profileForm.getRawValue())
      .pipe(finalize(() => (this.savingProfile = false)))
      .subscribe({
        next: (profile) => {
          this.profiles = [profile, ...this.profiles];
          this.quoteForm.patchValue({ supplierId: profile.id });
          this.productForm.patchValue({
            supplierId: profile.id,
            region: profile.region,
            city: profile.city
          });
          this.successMessage = 'Supplier profile submitted for verification.';
        },
        error: () => (this.errorMessage = 'Could not create supplier profile. Login may be required.')
      });
  }

  submitQuote(): void {
    if (this.quoteForm.invalid || this.savingQuote) {
      this.quoteForm.markAllAsTouched();
      return;
    }

    const { rfqId, ...payload } = this.quoteForm.getRawValue();
    if (!rfqId || !payload.supplierId) {
      this.errorMessage = 'Choose an RFQ and supplier profile.';
      return;
    }

    this.savingQuote = true;
    this.tradeService
      .createQuote(rfqId, payload)
      .pipe(finalize(() => (this.savingQuote = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Quote submitted.';
          this.load();
        },
        error: (error) => {
          this.errorMessage =
            error.status === 403
              ? 'This supplier profile does not belong to your account.'
              : 'Could not submit quote. Check the RFQ, supplier profile, unit price, and quantity.';
        }
      });
  }

  saveProduct(): void {
    if (this.productForm.invalid || this.savingProduct) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.savingProduct = true;
    const raw = this.productForm.getRawValue();
    const payload: ProductPayload = {
      ...raw,
      supplierId: raw.supplierId || null,
      description: raw.description || null
    };
    const request$ = this.editingProductId
      ? this.productService.update(this.editingProductId, payload)
      : this.productService.create(payload);

    request$.pipe(finalize(() => (this.savingProduct = false))).subscribe({
      next: (product) => {
        this.successMessage = this.editingProductId ? 'Product updated.' : 'Product added to your catalog.';
        this.products = this.editingProductId
          ? this.products.map((item) => (item.id === product.id ? product : item))
          : [product, ...this.products];
        this.resetProductForm();
      },
      error: () => (this.errorMessage = 'Could not save product. Login and supplier profile are required.')
    });
  }

  editProduct(product: Product): void {
    this.editingProductId = product.id;
    this.productForm.reset({
      supplierId: product.supplierId ?? '',
      name: product.name,
      description: product.description ?? '',
      category: product.category,
      price: product.price,
      unit: product.unit,
      stockQuantity: product.stockQuantity,
      availableQuantity: product.availableQuantity,
      region: product.region,
      city: product.city,
      isAvailable: product.isAvailable
    });
  }

  deleteProduct(productId: string): void {
    this.productService.delete(productId).subscribe({
      next: () => {
        this.products = this.products.filter((product) => product.id !== productId);
        this.successMessage = 'Product removed from catalog.';
      },
      error: () => (this.errorMessage = 'Could not delete product.')
    });
  }

  resetProductForm(): void {
    this.editingProductId = null;
    this.productForm.reset({
      supplierId: this.profiles[0]?.id ?? '',
      name: '',
      description: '',
      category: this.rfqs[0]?.category ?? '',
      price: 0,
      unit: 'pcs',
      stockQuantity: 0,
      availableQuantity: 0,
      region: this.profiles[0]?.region ?? '',
      city: this.profiles[0]?.city ?? '',
      isAvailable: true
    });
  }

  useRfq(rfq: Rfq): void {
    this.quoteForm.patchValue({ rfqId: rfq.id });
    this.productForm.patchValue({
      name: rfq.productName,
      category: rfq.category,
      unit: rfq.unit,
      region: rfq.deliveryRegion,
      city: rfq.deliveryCity
    });
  }
}

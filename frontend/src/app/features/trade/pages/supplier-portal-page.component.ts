import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { BusinessProfile, Rfq } from '../data-access/trade.models';
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

  profiles: BusinessProfile[] = [];
  rfqs: Rfq[] = [];
  loading = false;
  savingProfile = false;
  savingQuote = false;
  errorMessage = '';
  successMessage = '';

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

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    forkJoin({
      profiles: this.tradeService.getBusinessProfiles(),
      rfqs: this.tradeService.getRfqs()
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: ({ profiles, rfqs }) => {
          this.profiles = profiles;
          this.rfqs = rfqs;
          this.quoteForm.patchValue({
            supplierId: this.quoteForm.value.supplierId || profiles[0]?.id || '',
            rfqId: this.quoteForm.value.rfqId || rfqs[0]?.id || ''
          });
        },
        error: () => (this.errorMessage = 'Could not load supplier workspace.')
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
        next: () => (this.successMessage = 'Quote submitted.'),
        error: () => (this.errorMessage = 'Could not submit quote. Use a supplier profile you own.')
      });
  }
}

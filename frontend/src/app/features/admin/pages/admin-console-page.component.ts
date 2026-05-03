import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { Product, ProductPayload } from '../../products/data-access/product.models';
import { ProductService } from '../../products/data-access/product.service';
import { Category, City, Region } from '../data-access/admin.models';
import { AdminService } from '../data-access/admin.service';
import { BusinessProfile, Rfq } from '../../trade/data-access/trade.models';
import { AuthResponse, AuthService } from '../../trade/data-access/auth.service';
import { TradeService } from '../../trade/data-access/trade.service';

type AdminSection = 'overview' | 'verification' | 'profiles' | 'ratings' | 'trade' | 'settings';

@Component({
  selector: 'app-admin-console-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-console-page.component.html',
  styleUrl: './admin-console-page.component.css'
})
export class AdminConsolePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly adminService = inject(AdminService);
  private readonly authService = inject(AuthService);
  private readonly productService = inject(ProductService);
  private readonly tradeService = inject(TradeService);

  readonly sections: { id: AdminSection; label: string; icon: string }[] = [
    { id: 'overview', label: 'Overview', icon: 'dashboard' },
    { id: 'verification', label: 'Supplier Verification', icon: 'verified_user' },
    { id: 'profiles', label: 'Business Profiles', icon: 'badge' },
    { id: 'ratings', label: 'Rating Reports', icon: 'monitoring' },
    { id: 'trade', label: 'Marketplace & RFQs', icon: 'storefront' },
    { id: 'settings', label: 'Master Data', icon: 'tune' }
  ];
  readonly rfqStatuses = ['Open', 'Quoted', 'Awarded', 'Closed', 'Cancelled'];

  activeSection: AdminSection = 'overview';
  categories: Category[] = [];
  regions: Region[] = [];
  profiles: BusinessProfile[] = [];
  products: Product[] = [];
  rfqs: Rfq[] = [];
  currentUser: AuthResponse | null = null;
  loading = false;
  saving = false;
  errorMessage = '';
  successMessage = '';
  editingCategoryId: string | null = null;
  editingRegionId: string | null = null;
  editingCityId: string | null = null;
  editingProductId: string | null = null;
  editingProfileId: string | null = null;
  selectedProfileId = '';

  readonly categoryForm = this.fb.nonNullable.group({
    parentCategoryId: [''],
    name: ['', [Validators.required, Validators.maxLength(120)]],
    description: ['', [Validators.maxLength(500)]],
    sortOrder: [0, [Validators.required, Validators.min(0)]],
    isActive: [true]
  });

  readonly regionForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(120)]],
    code: ['', [Validators.maxLength(20)]],
    isActive: [true]
  });

  readonly cityForm = this.fb.nonNullable.group({
    regionId: [''],
    name: ['', [Validators.required, Validators.maxLength(120)]],
    isActive: [true]
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

  readonly profileForm = this.fb.nonNullable.group({
    businessName: ['', [Validators.required, Validators.maxLength(200)]],
    ownerName: ['', [Validators.required, Validators.maxLength(200)]],
    tinNumber: ['', [Validators.maxLength(50)]],
    phoneNumber: ['', [Validators.required, Validators.maxLength(50)]],
    region: ['', [Validators.required, Validators.maxLength(100)]],
    city: ['', [Validators.required, Validators.maxLength(100)]],
    address: ['', [Validators.maxLength(500)]],
    businessType: ['Supplier', [Validators.required, Validators.maxLength(100)]]
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
    description: ['', [Validators.required]],
    counterpartyName: [''],
    amount: [0],
    tradeDate: [new Date().toISOString().slice(0, 10)]
  });

  ngOnInit(): void {
    this.currentUser = this.authService.currentUser;
    this.authService.currentUser$.subscribe((user) => (this.currentUser = user));
    this.loadAll();
  }

  loadAll(): void {
    this.loading = true;
    this.errorMessage = '';
    forkJoin({
      categories: this.adminService.getCategories(),
      regions: this.adminService.getRegions(),
      profiles: this.tradeService.getBusinessProfiles(),
      products: this.productService.getAll(),
      rfqs: this.tradeService.getRfqs()
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: ({ categories, regions, profiles, products, rfqs }) => {
          this.categories = categories;
          this.regions = regions;
          this.profiles = profiles;
          this.products = products;
          this.rfqs = rfqs;
          this.setDefaults();
        },
        error: () => (this.errorMessage = 'Could not load admin workspace data.')
      });
  }

  activate(section: AdminSection): void {
    this.activeSection = section;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }

  get categoryRows(): { category: Category; depth: number }[] {
    const byParent = new Map<string, Category[]>();
    for (const category of this.categories) {
      const key = category.parentCategoryId ?? 'root';
      byParent.set(key, [...(byParent.get(key) ?? []), category]);
    }

    const rows: { category: Category; depth: number }[] = [];
    const visit = (parentId: string, depth: number) => {
      for (const category of byParent.get(parentId) ?? []) {
        rows.push({ category, depth });
        visit(category.id, depth + 1);
      }
    };
    visit('root', 0);
    return rows;
  }

  get cityOptions(): City[] {
    const selectedRegion = this.productForm.controls.region.value;
    return this.regions
      .filter((region) => !selectedRegion || region.name === selectedRegion)
      .flatMap((region) => region.cities);
  }

  get pendingProfiles(): BusinessProfile[] {
    return this.profiles.filter((profile) => profile.verificationStatus !== 'Verified');
  }

  get verifiedProfiles(): BusinessProfile[] {
    return this.profiles.filter((profile) => profile.verificationStatus === 'Verified');
  }

  get rejectedProfiles(): BusinessProfile[] {
    return this.profiles.filter((profile) => profile.verificationStatus === 'Rejected');
  }

  get topRatedProfiles(): BusinessProfile[] {
    return [...this.profiles]
      .filter((profile) => profile.ratingCount > 0)
      .sort((first, second) => second.ratingAverage - first.ratingAverage || second.tradeCount - first.tradeCount)
      .slice(0, 6);
  }

  get attentionProfiles(): BusinessProfile[] {
    return this.profiles
      .filter((profile) => profile.ratingCount === 0 || profile.ratingAverage < 3.5 || profile.verificationStatus !== 'Verified')
      .slice(0, 6);
  }

  get ratingAverage(): number {
    const ratedProfiles = this.profiles.filter((profile) => profile.ratingCount > 0);
    if (ratedProfiles.length === 0) {
      return 0;
    }

    return ratedProfiles.reduce((total, profile) => total + profile.ratingAverage, 0) / ratedProfiles.length;
  }

  get openRfqs(): Rfq[] {
    return this.rfqs.filter((rfq) => rfq.status === 'Open' || rfq.status === 'Quoted');
  }

  get liveProducts(): Product[] {
    return this.products.filter((product) => product.isAvailable);
  }

  get selectedProfile(): BusinessProfile | undefined {
    return this.profiles.find((profile) => profile.id === this.selectedProfileId);
  }

  saveCategory(): void {
    if (this.categoryForm.invalid || this.saving) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const raw = this.categoryForm.getRawValue();
    const payload = { ...raw, parentCategoryId: raw.parentCategoryId || null, description: raw.description || null };
    const request$ = this.editingCategoryId
      ? this.adminService.updateCategory(this.editingCategoryId, payload)
      : this.adminService.createCategory(payload);

    request$.pipe(finalize(() => (this.saving = false))).subscribe({
      next: () => {
        this.successMessage = 'Category saved.';
        this.resetCategoryForm();
        this.refreshTaxonomy();
      },
      error: () => (this.errorMessage = 'Could not save category.')
    });
  }

  editCategory(category: Category): void {
    this.editingCategoryId = category.id;
    this.categoryForm.reset({
      parentCategoryId: category.parentCategoryId ?? '',
      name: category.name,
      description: category.description ?? '',
      sortOrder: category.sortOrder,
      isActive: category.isActive
    });
  }

  deleteCategory(categoryId: string): void {
    this.adminService.deleteCategory(categoryId).subscribe({
      next: () => this.refreshTaxonomy(),
      error: () => (this.errorMessage = 'Could not delete category. Remove child categories first.')
    });
  }

  resetCategoryForm(): void {
    this.editingCategoryId = null;
    this.categoryForm.reset({ parentCategoryId: '', name: '', description: '', sortOrder: 0, isActive: true });
  }

  saveRegion(): void {
    if (this.regionForm.invalid || this.saving) {
      this.regionForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const raw = this.regionForm.getRawValue();
    const payload = { ...raw, code: raw.code || null };
    const request$ = this.editingRegionId
      ? this.adminService.updateRegion(this.editingRegionId, payload)
      : this.adminService.createRegion(payload);

    request$.pipe(finalize(() => (this.saving = false))).subscribe({
      next: () => {
        this.successMessage = 'Region saved.';
        this.resetRegionForm();
        this.refreshLocations();
      },
      error: () => (this.errorMessage = 'Could not save region.')
    });
  }

  editRegion(region: Region): void {
    this.editingRegionId = region.id;
    this.regionForm.reset({ name: region.name, code: region.code ?? '', isActive: region.isActive });
  }

  deleteRegion(regionId: string): void {
    this.adminService.deleteRegion(regionId).subscribe({
      next: () => this.refreshLocations(),
      error: () => (this.errorMessage = 'Could not delete region.')
    });
  }

  resetRegionForm(): void {
    this.editingRegionId = null;
    this.regionForm.reset({ name: '', code: '', isActive: true });
  }

  saveCity(): void {
    if (this.cityForm.invalid || this.saving) {
      this.cityForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const payload = this.cityForm.getRawValue();
    const request$ = this.editingCityId
      ? this.adminService.updateCity(this.editingCityId, payload)
      : this.adminService.createCity(payload);

    request$.pipe(finalize(() => (this.saving = false))).subscribe({
      next: () => {
        this.successMessage = 'City saved.';
        this.resetCityForm();
        this.refreshLocations();
      },
      error: () => (this.errorMessage = 'Could not save city.')
    });
  }

  editCity(city: City): void {
    this.editingCityId = city.id;
    this.cityForm.reset({ regionId: city.regionId, name: city.name, isActive: city.isActive });
  }

  deleteCity(cityId: string): void {
    this.adminService.deleteCity(cityId).subscribe({
      next: () => this.refreshLocations(),
      error: () => (this.errorMessage = 'Could not delete city.')
    });
  }

  resetCityForm(): void {
    this.editingCityId = null;
    this.cityForm.reset({ regionId: this.regions[0]?.id ?? '', name: '', isActive: true });
  }

  saveProduct(): void {
    if (this.productForm.invalid || this.saving) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const raw = this.productForm.getRawValue();
    const payload: ProductPayload = { ...raw, supplierId: raw.supplierId || null, description: raw.description || null };
    const request$ = this.editingProductId
      ? this.productService.update(this.editingProductId, payload)
      : this.productService.create(payload);

    request$.pipe(finalize(() => (this.saving = false))).subscribe({
      next: () => {
        this.successMessage = 'Product saved.';
        this.resetProductForm();
        this.refreshProducts();
      },
      error: () => (this.errorMessage = 'Could not save product.')
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
      next: () => this.refreshProducts(),
      error: () => (this.errorMessage = 'Could not delete product.')
    });
  }

  selectProfile(profile: BusinessProfile): void {
    this.selectedProfileId = profile.id;
    this.verificationForm.patchValue({
      supplierId: profile.id,
      verificationStatus: profile.verificationStatus
    });
    this.ratingForm.patchValue({ supplierId: profile.id });
    this.historyForm.patchValue({ supplierId: profile.id });
  }

  editProfile(profile: BusinessProfile): void {
    this.selectProfile(profile);
    this.editingProfileId = profile.id;
    this.profileForm.reset({
      businessName: profile.businessName,
      ownerName: profile.ownerName,
      tinNumber: profile.tinNumber,
      phoneNumber: profile.phoneNumber,
      region: profile.region,
      city: profile.city,
      address: profile.address ?? '',
      businessType: profile.businessType
    });
  }

  saveProfile(): void {
    if (this.profileForm.invalid || this.saving) {
      this.profileForm.markAllAsTouched();
      return;
    }

    if (!this.editingProfileId) {
      this.errorMessage = 'Choose a profile to edit first.';
      return;
    }

    this.saving = true;
    const raw = this.profileForm.getRawValue();
    this.tradeService
      .updateBusinessProfile(this.editingProfileId, { ...raw, address: raw.address || null })
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (profile) => {
          this.successMessage = 'Business profile updated.';
          this.profiles = this.profiles.map((item) => (item.id === profile.id ? profile : item));
          this.selectProfile(profile);
        },
        error: () => (this.errorMessage = 'Could not update business profile.')
      });
  }

  resetProfileForm(): void {
    this.editingProfileId = null;
    this.profileForm.reset({
      businessName: '',
      ownerName: '',
      tinNumber: '',
      phoneNumber: '',
      region: this.regions[0]?.name ?? '',
      city: this.regions[0]?.cities[0]?.name ?? '',
      address: '',
      businessType: 'Supplier'
    });
  }

  resetProductForm(): void {
    this.editingProductId = null;
    this.productForm.reset({
      supplierId: '',
      name: '',
      description: '',
      category: this.categories[0]?.name ?? '',
      price: 0,
      unit: 'pcs',
      stockQuantity: 0,
      availableQuantity: 0,
      region: this.regions[0]?.name ?? '',
      city: this.regions[0]?.cities[0]?.name ?? '',
      isAvailable: true
    });
  }

  verifyProfile(): void {
    const { supplierId, verificationStatus } = this.verificationForm.getRawValue();
    if (!supplierId || this.saving) {
      return;
    }

    this.saving = true;
    this.tradeService
      .verifyBusinessProfile(supplierId, verificationStatus)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Profile verification updated.';
          this.refreshProfiles();
        },
        error: () => (this.errorMessage = 'Could not update profile verification.')
      });
  }

  updateRfqStatus(rfq: Rfq, status: string): void {
    this.adminService.updateRfqStatus(rfq.id, status).subscribe({
      next: (updated) => {
        this.rfqs = this.rfqs.map((item) => (item.id === updated.id ? updated : item));
        this.successMessage = 'RFQ status updated.';
      },
      error: () => (this.errorMessage = 'Could not update RFQ status.')
    });
  }

  createRating(): void {
    const { supplierId, ...payload } = this.ratingForm.getRawValue();
    if (!supplierId || this.ratingForm.invalid || this.saving) {
      this.ratingForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.tradeService
      .createRating(supplierId, payload)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'TradeTrust rating added.';
          this.refreshProfiles();
        },
        error: () => (this.errorMessage = 'Could not add rating.')
      });
  }

  createHistory(): void {
    const { supplierId, ...rawPayload } = this.historyForm.getRawValue();
    if (!supplierId || !rawPayload.description || this.saving) {
      this.historyForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.tradeService
      .createTradeHistory(supplierId, {
        ...rawPayload,
        amount: rawPayload.amount || null,
        tradeDate: new Date(rawPayload.tradeDate).toISOString()
      })
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Trade history added.';
          this.refreshProfiles();
        },
        error: () => (this.errorMessage = 'Could not add trade history.')
      });
  }

  private refreshTaxonomy(): void {
    this.adminService.getCategories().subscribe((categories) => (this.categories = categories));
  }

  private refreshLocations(): void {
    this.adminService.getRegions().subscribe((regions) => {
      this.regions = regions;
      this.setDefaults();
    });
  }

  private refreshProducts(): void {
    this.productService.getAll().subscribe((products) => (this.products = products));
  }

  private refreshProfiles(): void {
    this.tradeService.getBusinessProfiles().subscribe((profiles) => {
      this.profiles = profiles;
      this.setDefaults();
    });
  }

  private setDefaults(): void {
    const firstProfileId = this.profiles[0]?.id ?? '';
    this.selectedProfileId = this.selectedProfileId || firstProfileId;
    this.verificationForm.patchValue({ supplierId: this.verificationForm.value.supplierId || firstProfileId });
    this.ratingForm.patchValue({ supplierId: this.ratingForm.value.supplierId || firstProfileId });
    this.historyForm.patchValue({ supplierId: this.historyForm.value.supplierId || firstProfileId });
    this.cityForm.patchValue({ regionId: this.cityForm.value.regionId || this.regions[0]?.id || '' });
    this.productForm.patchValue({
      category: this.productForm.value.category || this.categories[0]?.name || '',
      region: this.productForm.value.region || this.regions[0]?.name || '',
      city: this.productForm.value.city || this.regions[0]?.cities[0]?.name || ''
    });
  }
}

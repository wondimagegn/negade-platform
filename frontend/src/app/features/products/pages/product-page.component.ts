import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { Product, ProductPayload } from '../data-access/product.models';
import { ProductService } from '../data-access/product.service';

@Component({
  selector: 'app-product-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.css'
})
export class ProductPageComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  submitting = false;
  errorMessage = '';
  editingId: string | null = null;

  form: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly productService: ProductService
  ) {
    this.form = this.fb.nonNullable.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(2000)]],
      price: [0, [Validators.required, Validators.min(0)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    console.log('ProductPageComponent initialized');
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';
    this.productService
      .getAll()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (products) => (this.products = products),
        error: () => (this.errorMessage = 'Failed to load products.')
      });
  }

  save(): void {
    if (this.form.invalid || this.submitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    const payload: ProductPayload = this.form.getRawValue();
    const request$ = this.editingId
      ? this.productService.update(this.editingId, payload)
      : this.productService.create(payload);

    request$.pipe(finalize(() => (this.submitting = false))).subscribe({
      next: () => {
        this.cancelEdit();
        this.loadProducts();
      },
      error: () => (this.errorMessage = 'Failed to save product.')
    });
  }

  startEdit(product: Product): void {
    this.editingId = product.id;
    this.form.patchValue({
      name: product.name,
      description: product.description ?? '',
      price: product.price,
      stockQuantity: product.stockQuantity
    });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.form.reset({
      name: '',
      description: '',
      price: 0,
      stockQuantity: 0
    });
  }

  remove(productId: string): void {
    if (this.submitting) {
      return;
    }

    this.submitting = true;
    this.productService
      .delete(productId)
      .pipe(finalize(() => (this.submitting = false)))
      .subscribe({
        next: () => this.loadProducts(),
        error: () => (this.errorMessage = 'Failed to delete product.')
      });
  }
}

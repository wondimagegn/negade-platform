export interface Product {
  id: string;
  supplierId?: string | null;
  supplierName?: string | null;
  name: string;
  description?: string | null;
  category: string;
  price: number;
  unit: string;
  stockQuantity: number;
  availableQuantity: number;
  region: string;
  city: string;
  isAvailable: boolean;
  createdAt: string;
}

export interface ProductPayload {
  supplierId?: string | null;
  name: string;
  description?: string | null;
  category: string;
  price: number;
  unit: string;
  stockQuantity: number;
  availableQuantity: number;
  region: string;
  city: string;
  isAvailable: boolean;
}

export interface Product {
  id: string;
  name: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  createdAt: string;
}

export interface ProductPayload {
  name: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
}

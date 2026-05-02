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

export interface BusinessProfile {
  id: string;
  businessName: string;
  ownerName: string;
  tinNumber: string;
  phoneNumber: string;
  region: string;
  city: string;
  address?: string | null;
  businessType: string;
  verificationStatus: string;
  ratingAverage: number;
  ratingCount: number;
  tradeCount: number;
  createdAt: string;
}

export interface BusinessProfilePayload {
  businessName: string;
  ownerName: string;
  tinNumber: string;
  phoneNumber: string;
  region: string;
  city: string;
  address?: string | null;
  businessType: string;
}

export interface Rfq {
  id: string;
  buyerName: string;
  buyerPhoneNumber: string;
  buyerBusinessName?: string | null;
  productName: string;
  category: string;
  quantity: number;
  unit: string;
  deliveryRegion: string;
  deliveryCity: string;
  notes?: string | null;
  status: string;
  quoteCount: number;
  createdAt: string;
}

export interface RfqPayload {
  buyerName: string;
  buyerPhoneNumber: string;
  buyerBusinessName?: string | null;
  productName: string;
  category: string;
  quantity: number;
  unit: string;
  deliveryRegion: string;
  deliveryCity: string;
  notes?: string | null;
}

export interface ProductSearchParams {
  search?: string;
  category?: string;
  region?: string;
}

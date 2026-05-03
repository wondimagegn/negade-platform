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

export type UpdateBusinessProfilePayload = BusinessProfilePayload;

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

export interface Quote {
  id: string;
  rfqId: string;
  supplierId: string;
  supplierUserId?: string | null;
  supplierName: string;
  unitPrice: number;
  quantityAvailable: number;
  deliveryTimeInDays: number;
  notes?: string | null;
  createdAt: string;
}

export interface QuotePayload {
  supplierId: string;
  unitPrice: number;
  quantityAvailable: number;
  deliveryTimeInDays: number;
  notes?: string | null;
}

export interface TradeRating {
  id: string;
  supplierId: string;
  raterUserId?: string | null;
  score: number;
  comment?: string | null;
  createdAt: string;
}

export interface TradeRatingPayload {
  score: number;
  comment?: string | null;
}

export interface TradeHistory {
  id: string;
  businessProfileId: string;
  description: string;
  counterpartyName?: string | null;
  amount?: number | null;
  tradeDate: string;
  createdAt: string;
}

export interface TradeHistoryPayload {
  description: string;
  counterpartyName?: string | null;
  amount?: number | null;
  tradeDate: string;
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

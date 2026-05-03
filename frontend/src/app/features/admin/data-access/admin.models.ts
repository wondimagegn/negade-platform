export interface Category {
  id: string;
  parentCategoryId?: string | null;
  name: string;
  description?: string | null;
  sortOrder: number;
  isActive: boolean;
  createdAt: string;
}

export interface CategoryPayload {
  parentCategoryId?: string | null;
  name: string;
  description?: string | null;
  sortOrder: number;
  isActive: boolean;
}

export interface City {
  id: string;
  regionId: string;
  name: string;
  isActive: boolean;
  createdAt: string;
}

export interface Region {
  id: string;
  name: string;
  code?: string | null;
  isActive: boolean;
  createdAt: string;
  cities: City[];
}

export interface RegionPayload {
  name: string;
  code?: string | null;
  isActive: boolean;
}

export interface CityPayload {
  regionId: string;
  name: string;
  isActive: boolean;
}

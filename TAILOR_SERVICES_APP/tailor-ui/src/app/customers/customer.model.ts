export interface CustomerGridDto {
  id: string;
  name: string;
  phone: string | null;
  address: string | null;
  createdAt: string; // DateTime arrives as an ISO string
}

export interface CustomerDetailDto {
  id: string;
  name: string;
  phone: string | null;
  address: string | null;
  notes: string | null;
}

export interface CustomerSaveDto {
  name: string;
  phone: string | null;
  address: string | null;
  notes: string | null;
}

export interface SortDescriptor {
  field: string;
  dir: 'asc' | 'desc';
}

export interface FilterDescriptor {
  logic?: 'and' | 'or';
  field?: string;
  operator?: 'eq' | 'neq' | 'lt' | 'lte' | 'gt' | 'gte'
  | 'contains' | 'doesnotcontain' | 'startswith' | 'endswith'
  | 'isnull' | 'isnotnull';
  value?: string | number | boolean | null;
  filters?: FilterDescriptor[];
}

export interface GridRequest {
  page: number;
  pageSize: number;
  sorts?: SortDescriptor[];
  filter?: FilterDescriptor;
}

export interface GridResult<T> {
  data: T[];
  total: number;
}

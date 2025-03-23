// Enum para status da venda
export type SaleStatus =
  | 'Pending'
  | 'Processing'
  | 'Confirmed'
  | 'Completed'
  | 'Shipped'
  | 'Delivered'
  | 'Cancelled'
  | 'Failed'
  | 'Refunded';

// Enum para status do item da venda
export type SaleItemStatus =
  | 'Active'
  | 'Cancelled'
  | 'Returned'
  | 'OutOfStock'
  | 'Shipped'
  | 'Delivered';

// ==========================
// Models principais de venda
// ==========================

// Venda completa
export interface Sale {
  id: number;
  saleNumber: string;
  saleDate: string;
  customerId: number;
  customerName: string;
  branch: string;
  status: SaleStatus;
  totalValue: Money;
  items: SaleItem[];
}

// Item da venda
export interface SaleItem {
  id: number;
  saleId: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: Money;
  discount: Money;
  total: Money;
  status: SaleItemStatus;
}

// ==========================
// DTOs de requisição
// ==========================

export interface CancelSaleRequest {
  saleId: number;
}

export interface CancelItemRequest {
  saleId: number;
  productId: number;
}

export interface UpdateSaleRequest {
  customerId: number;
  items: SaleItemRequest[];
}

export interface SaleItemRequest {
  productId: number;
  quantity: number;
}

export interface GetSaleRequest {
  id: number;
}

// ==========================
// DTOs de resposta
// ==========================

export interface CancelSaleResponse {
  saleId: number;
  saleNumber: string;
  saleDate: string;
  customerId: number;
  customerName: string;
  totalValue: number;
  branch: string;
  status: SaleStatus;
  items: SaleItemResponse[];
}

export interface CancelItemResponse {
  id: number;
  saleId: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  total: number;
  status: SaleItemStatus;
}

export interface UpdateSaleResponse {
  saleId: number;
  saleNumber: string;
  saleDate: string;
  customerId: number;
  customerName: string;
  totalValue: number;
  branch: string;
  status: SaleStatus;
  items: SaleItemResponse[];
}

export interface GetSaleResponse {
  saleId: number;
  saleNumber: string;
  saleDate: string;
  customerId: number;
  customerName: string;
  totalValue: number;
  branch: string;
  status: SaleStatus;
  items: SaleItemResponse[];
}

export interface GetSaleByIdResponse {
  saleId: number;
  saleNumber: string;
  saleDate: string;
  customerId: number;
  customerName: string;
  totalValue: number;
  branch: string;
  status: string;
  items: {
    id: number;
    productId: number;
    productName: string;
    quantity: number;
    unitPrice: number;
    discount: number;
    total: number;
    status: string;
  }[];
}


export interface SaleItemResponse {
  id: number;
  saleId: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  total: number;
  status: SaleItemStatus;
}

// ==========================
// DTO simplificado para listagem
// ==========================

export interface GetSaleResult {
  saleId: number;
  saleNumber: string;
  saleDate: string;
  customerId: number;
  customerName: string;
  totalValue: number;
  branch: string;
  status: SaleStatus;
  items: SaleItemResult[];
}

export interface SaleItemResult {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  total: number;
  status: SaleItemStatus;
}

export interface Money {
  amount: number;
  currency: string;
}


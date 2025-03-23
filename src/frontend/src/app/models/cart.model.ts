export interface CartItem {
  productId: number;
  quantity: number;
  unitPrice: number;
  discount: number;
  total: number;
}

export interface CartRequest {
  userId: number;
  date: string; // ISO string format
  products: CartItem[];
}

export interface CreateCartRequest {
  userId: number;
  date: string;
  products: CartItem[];
}

export interface UpdateCartRequest {
  userId: number;
  date: string;
  products: CartItem[];
}

export interface GetCartRequest {
  id: number;
}

export interface DeleteCartRequest {
  id: number;
}

export interface CheckoutRequest {
  cartId: number;
}

export interface CartResponse {
  id: number;
  userId: number;
  date: string;
  status: CartStatus;
  products: CartItem[];
  totalPrice: number;
}

export enum CartStatus {
  Active = 'Active',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

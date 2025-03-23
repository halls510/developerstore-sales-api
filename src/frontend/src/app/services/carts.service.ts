import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

import { CartRequest, CartResponse } from '../models/cart.model';
import { PaginatedResponse } from '../models/paginated-response.model';
import { PaginationQuery } from '../models/pagination-query.model';
import { ApiResponseWithData } from '../models/api-response-with-data.model';

const API_URL = `${environment.apiUrl}/carts`;

@Injectable({
  providedIn: 'root'
})
export class CartsService {
  constructor(private http: HttpClient) {}

  // 📄 Lista de carrinhos com paginação e filtros
  getCarts(query?: PaginationQuery): Observable<PaginatedResponse<CartResponse>> {
    const params = this.buildQueryParams(query);
    return this.http.get<PaginatedResponse<CartResponse>>(API_URL, { params });
  }

  // 🔍 Detalhe do carrinho por ID
  getCartById(id: number): Observable<ApiResponseWithData<CartResponse>> {
    return this.http.get<ApiResponseWithData<CartResponse>>(`${API_URL}/${id}`);
  }

  // 🔍 Buscar carrinho por userId via filtro
  getCartByUserId(userId: number): Observable<ApiResponseWithData<CartResponse[]>> {
    const params = new HttpParams().append('UserId', userId.toString());
    return this.http.get<ApiResponseWithData<CartResponse[]>>(API_URL, { params });
  }

  // ➕ Criar novo carrinho
  createCart(cart: CartRequest): Observable<ApiResponseWithData<CartResponse>> {
    return this.http.post<ApiResponseWithData<CartResponse>>(API_URL, cart);
  }

  // ✏️ Atualizar carrinho existente
  updateCart(id: number, cart: CartRequest): Observable<ApiResponseWithData<CartResponse>> {
    return this.http.put<ApiResponseWithData<CartResponse>>(`${API_URL}/${id}`, cart);
  }

  // ❌ Remover carrinho
  deleteCart(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${id}`);
  }

  // ✅ Finalizar (checkout) carrinho
  checkoutCart(id: number): Observable<void> {
    return this.http.post<void>(`${API_URL}/${id}/checkout`, null);
  }

  // ⚙️ Monta os parâmetros para paginação, filtros e ordenação
  private buildQueryParams(query?: PaginationQuery): HttpParams {
    let params = new HttpParams();

    if (!query) return params;

    if (query._page != null) {
      params = params.set('_page', query._page.toString());
    }

    if (query._size != null) {
      params = params.set('_size', query._size.toString());
    }

    if (query._order) {
      params = params.set('_order', query._order);
    }

    if (query.filters) {
      for (const key in query.filters) {
        const values = query.filters[key];
        if (Array.isArray(values)) {
          values.forEach(value => {
            params = params.append(key, value);
          });
        }
      }
    }

    return params;
  }
}

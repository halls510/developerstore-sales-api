import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  GetSaleResult,
  GetSaleByIdResponse,
  UpdateSaleRequest,
  UpdateSaleResponse,
  CancelSaleRequest,
  CancelSaleResponse,
  CancelItemRequest,
  CancelItemResponse
} from 'src/app/models/sale.model';
import { environment } from 'src/environments/environment';
import { PaginatedResponse } from 'src/app/models/paginated-response.model';
import { PaginationQuery } from 'src/app/models/pagination-query.model';

const API_URL = `${environment.apiUrl}/sales`;

@Injectable({
  providedIn: 'root'
})
export class SalesService {
  constructor(private http: HttpClient) {}

  // 📄 Lista de vendas com paginação e filtros
  getSales(query?: PaginationQuery): Observable<PaginatedResponse<GetSaleResult>> {
    const params = this.buildQueryParams(query);
    return this.http.get<PaginatedResponse<GetSaleResult>>(API_URL, { params });
  }

  // 🔍 Detalhe de uma venda por ID
  getSaleById(id: number): Observable<GetSaleByIdResponse> {
    return this.http.get<GetSaleByIdResponse>(`${API_URL}/${id}`);
  }

  // ✏️ Atualização de venda
  updateSale(id: number, sale: UpdateSaleRequest): Observable<UpdateSaleResponse> {
    return this.http.put<UpdateSaleResponse>(`${API_URL}/${id}`, sale);
  }

  // ❌ Cancelamento total da venda
  cancelSale(saleId: number): Observable<CancelSaleResponse> {
    const payload: CancelSaleRequest = { saleId };
    return this.http.patch<CancelSaleResponse>(`${API_URL}/${saleId}/cancel`, payload);
  }

  // ❌ Cancelamento de item
  cancelItem(payload: CancelItemRequest): Observable<CancelItemResponse> {
    return this.http.patch<CancelItemResponse>(
      `${API_URL}/${payload.saleId}/items/${payload.productId}/cancel`,
      payload
    );
  }

  // 👤 Vendas do usuário autenticado
  getMySales(): Observable<GetSaleResult[]> {
    return this.http.get<GetSaleResult[]>(`${API_URL}/my`);
  }

  // ⚙️ Gera HttpParams conforme estrutura esperada pela API
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

    console.log(query);

    if (query.filters) {
      for (const key in query.filters) {
        const values = query.filters[key];
        if (Array.isArray(values)) {
          values.forEach(value => {
            params = params.append(`${key}`, value);
          });
        }
      }
    }

    return params;
  }
}
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

import {
  Product,
  ProductRequest,
  ProductResponse
} from '../models/product.model';
import { CategoriesResponse } from '../models/category.model';
import { PaginatedResponse } from '../models/paginated-response.model';
import { PaginationQuery } from '../models/pagination-query.model';
import { ApiResponseWithData } from '../models/api-response-with-data.model';

const API_URL = `${environment.apiUrl}/products`;

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  constructor(private http: HttpClient) {}

  // 📄 Lista de produtos com paginação e filtros
  getProducts(query?: PaginationQuery): Observable<PaginatedResponse<ProductResponse>> {
    const params = this.buildQueryParams(query);
    return this.http.get<PaginatedResponse<ProductResponse>>(API_URL, { params });
  }

  // 📄 Lista de produtos por categoria com paginação e filtros
  getProductsByCategory(category: string, query?: PaginationQuery): Observable<PaginatedResponse<ProductResponse>> {
    const params = this.buildQueryParams(query);
    return this.http.get<PaginatedResponse<ProductResponse>>(`${API_URL}/category/${category}`, { params });
  }

  // 🗂️ Lista de categorias
  getCategories(): Observable<CategoriesResponse> {
    return this.http.get<CategoriesResponse>(`${API_URL}/categories`);
  }

  // 🔍 Detalhe do produto
  getProductById(id: number): Observable<ApiResponseWithData<Product>> {
    return this.http.get<ApiResponseWithData<Product>>(`${API_URL}/${id}`);
  }

  // ➕ Criação de produto
  createProduct(product: ProductRequest): Observable<ApiResponseWithData<Product>> {
    return this.http.post<ApiResponseWithData<Product>>(API_URL, product);
  }

  // ✏️ Atualização de produto
  updateProduct(id: number, product: ProductRequest): Observable<ApiResponseWithData<Product>> {
    return this.http.put<ApiResponseWithData<Product>>(`${API_URL}/${id}`, product);
  }

  // ❌ Remoção de produto
  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${id}`);
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



// import { Injectable } from '@angular/core';
// import { HttpClient, HttpParams } from '@angular/common/http';
// import { Observable } from 'rxjs';
// import { environment } from 'src/environments/environment';

// import {
//   Product,
//   ProductRequest,
//   ProductResponse
// } from '../models/product.model';
// import { CategoriesResponse } from '../models/category.model';
// import { PaginatedResponse } from '../models/paginated-response.model';
// import { PaginationQuery } from '../models/pagination-query.model';

// const API_URL = `${environment.apiUrl}/products`;

// @Injectable({
//   providedIn: 'root'
// })
// export class ProductsService {
//   constructor(private http: HttpClient) {}

//   // 📄 Lista de produtos com paginação e filtros
//   getProducts(query?: PaginationQuery): Observable<PaginatedResponse<ProductResponse>> {
//     const params = this.buildQueryParams(query);
//     return this.http.get<PaginatedResponse<ProductResponse>>(API_URL, { params });
//   }

//   // 📄 Lista de produtos por categoria com paginação e filtros
//   getProductsByCategory(category: string, query?: PaginationQuery): Observable<PaginatedResponse<ProductResponse>> {
//     const params = this.buildQueryParams(query);
//     return this.http.get<PaginatedResponse<ProductResponse>>(`${API_URL}/category/${category}`, { params });
//   }

//   // 🗂️ Lista de categorias
//   getCategories(): Observable<CategoriesResponse> {
//     return this.http.get<CategoriesResponse>(`${API_URL}/categories`);
//   }

//   // 🔍 Detalhe do produto
//   getProductById(id: number): Observable<ProductResponse> {
//     return this.http.get<ProductResponse>(`${API_URL}/${id}`);
//   }

//   // ➕ Criação de produto
//   createProduct(product: ProductRequest): Observable<ProductResponse> {
//     return this.http.post<ProductResponse>(API_URL, product);
//   }

//   // ✏️ Atualização de produto
//   updateProduct(id: number, product: ProductRequest): Observable<ProductResponse> {
//     return this.http.put<ProductResponse>(`${API_URL}/${id}`, product);
//   }

//   // ❌ Remoção de produto
//   deleteProduct(id: number): Observable<void> {
//     return this.http.delete<void>(`${API_URL}/${id}`);
//   }

//   // ⚙️ Gera HttpParams conforme estrutura esperada pela API
//   private buildQueryParams(query?: PaginationQuery): HttpParams {
//     let params = new HttpParams();

//     if (!query) return params;

//     if (query._page != null) {
//       params = params.set('_page', query._page.toString());
//     }

//     if (query._size != null) {
//       params = params.set('_size', query._size.toString());
//     }

//     if (query._order) {
//       params = params.set('_order', query._order);
//     }

//     if (query.filters) {
//       for (const key in query.filters) {
//         const values = query.filters[key];
//         if (Array.isArray(values)) {
//           values.forEach(value => {        
//             params = params.append(`${key}`, value);
//           });
//         }
//       }
//     }

//     return params;
//   }
// }


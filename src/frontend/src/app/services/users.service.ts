import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  GetUserResponse,
  CreateUserRequest,
  CreateUserResponse,
  UpdateUserRequest
} from 'src/app/models/user.model';
import { environment } from 'src/environments/environment';
import { PaginatedResponse } from 'src/app/models/paginated-response.model';
import { PaginationQuery } from 'src/app/models/pagination-query.model';

const API_URL = `${environment.apiUrl}/users`;

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  constructor(private http: HttpClient) {}

  // 📄 Lista de usuários com paginação e filtros
  getUsers(query?: PaginationQuery): Observable<PaginatedResponse<GetUserResponse>> {
    const params = this.buildQueryParams(query);
    console.log(params);
    return this.http.get<PaginatedResponse<GetUserResponse>>(API_URL, { params });
  }

  // 🔍 Detalhe de um usuário por ID
  getUserById(id: number): Observable<GetUserResponse> {
    return this.http.get<GetUserResponse>(`${API_URL}/${id}`);
  }

  // ➕ Criação de usuário
  createUser(user: CreateUserRequest): Observable<CreateUserResponse> {
    return this.http.post<CreateUserResponse>(API_URL, user);
  }

  // ✏️ Atualização de usuário
  updateUser(id: number, user: UpdateUserRequest): Observable<CreateUserResponse> {
    return this.http.put<CreateUserResponse>(`${API_URL}/${id}`, user);
  }

  // ❌ Exclusão de usuário
  deleteUser(id: number): Observable<CreateUserResponse> {
    return this.http.delete<CreateUserResponse>(`${API_URL}/${id}`);
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
            params = params.append(`${key}`, value);
          });
        }
      }
    }

    return params;
  }
}

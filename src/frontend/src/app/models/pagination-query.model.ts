// src/app/models/query-params.model.ts
export interface PaginationQuery {
  _page?: number;
  _size?: number;
  _order?: string;
  filters?: { [key: string]: string[] }; // múltiplos valores por chave
}

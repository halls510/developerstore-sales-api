import { Component, OnInit } from '@angular/core';
import { SalesService } from 'src/app/services/sales.service';
import {
  CancelSaleResponse,
  GetSaleResult
} from 'src/app/models/sale.model';
import { PaginationQuery } from 'src/app/models/pagination-query.model';

@Component({
  selector: 'app-sales-list',
  templateUrl: './sales-list.component.html',
  styleUrls: ['./sales-list.component.scss']
})
export class SalesListComponent implements OnInit {
  sales: GetSaleResult[] = [];
  selectedSale?: GetSaleResult;

  // UI State
  drawerVisible = false;
  isLoading = false;
  errorMessage: string | null = null;

  // Paginação
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  totalCount = 0;

  // Ordenação
  orderBy?: string = "";

  // Filtros
  searchQuery: string = '';
  selectedStatus: string = 'Todos';

  constructor(private salesService: SalesService) {}

  ngOnInit(): void {
    this.loadSales();
  }

  /**
   * Carrega as vendas com base em filtros, ordenação e paginação
   */
  loadSales(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const filters: { [key: string]: string[] } = {};
    if (this.searchQuery) filters['CustomerName'] = [`*${this.searchQuery}*`];
    if (this.selectedStatus !== 'Todos') filters['status'] = [this.selectedStatus];

    const query: PaginationQuery = {
      _page: this.currentPage,
      _size: this.pageSize,
      _order: this.orderBy,
      filters
    };

    console.log(query);
    this.salesService.getSales(query).subscribe({
      next: (res) => {
        this.sales = res.data;
        this.totalPages = res.totalPages;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Erro ao carregar as vendas.';
        console.error('[Erro] Buscar vendas:', err);
        this.isLoading = false;
      }
    });
  }

  /**
   * Altera a página atual
   */
  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadSales();
  }

  /**
   * Ordena por campo
   */
  sort(by: string, direction: 'asc' | 'desc' = 'asc'): void {
    this.orderBy = `${by} ${direction}`;
    this.loadSales();
  }

  /**
   * Executa a pesquisa com o termo atual
   */
  onSearch(): void {
    this.currentPage = 1;
    this.loadSales();
  }

  /**
   * Executa a pesquisa ao mudar o status
   */
  onStatusChange(): void {
    this.currentPage = 1;
    this.loadSales();
  }

  /**
   * Exibe os detalhes da venda
   */
  viewDetails(sale: GetSaleResult): void {
    this.selectedSale = sale;
    this.drawerVisible = true;
  }

  /**
   * Fecha o drawer
   */
  closeDrawer(): void {
    this.drawerVisible = false;
  }

  /**
   * Cancela uma venda
   */
  cancelSale(saleId: number): void {
    const confirmCancel = confirm('Tem certeza que deseja cancelar esta venda?');
    if (!confirmCancel) return;

    this.salesService.cancelSale(saleId).subscribe({
      next: (response: CancelSaleResponse) => {
        this.sales = this.sales.map(sale =>
          sale.saleId === response.saleId
            ? { ...sale, status: response.status }
            : sale
        );
      },
      error: (err) => {
        this.errorMessage = 'Erro ao cancelar a venda.';
        console.error('[Erro] Cancelar venda:', err);
      }
    });
  }
}
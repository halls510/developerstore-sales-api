import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SalesService } from 'src/app/services/sales.service';
import { GetSaleByIdResponse } from 'src/app/models/sale.model';

@Component({
  selector: 'app-sale-details',
  templateUrl: './sale-details.component.html',
  styleUrls: ['./sale-details.component.scss']
})
export class SaleDetailsComponent implements OnInit {
  sale: GetSaleByIdResponse | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private salesService: SalesService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (isNaN(id)) {
      this.errorMessage = 'ID de venda inválido.';
      this.isLoading = false;
      return;
    }

    this.salesService.getSaleById(id).subscribe({
      next: (data) => {
        this.sale = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Erro ao carregar os detalhes da venda.';
        this.isLoading = false;
      }
    });
  }
}

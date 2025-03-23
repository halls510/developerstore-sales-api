import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalesListComponent } from './sales-list/sales-list.component';
import { SaleDetailsComponent } from './sale-details/sale-details.component';
import { SaleDetailsDrawerComponent } from './sale-details-drawer/sale-details-drawer.component';
import { RouterModule } from '@angular/router';
import { SalesManagementRoutingModule } from './sales-management-routing.module';
import { FormsModule } from '@angular/forms'; // ✅ Importar FormsModule aqui

@NgModule({
  declarations: [
    SalesListComponent,
    SaleDetailsComponent,
    SaleDetailsDrawerComponent
  ],
  imports: [
    CommonModule,
    SalesManagementRoutingModule,
    RouterModule,
    FormsModule
  ]
})
export class SalesManagementModule {}

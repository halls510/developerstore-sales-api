import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SalesRoutingModule } from './sales-routing.module';
import { SalesComponent } from './sales.component';
import { SalesListComponent } from './sales-list/sales-list.component';
import { MySalesComponent } from './my-sales/my-sales.component';
import { SaleDetailsComponent } from './sale-details/sale-details.component';


@NgModule({
  declarations: [
    SalesComponent,
    SalesListComponent,
    MySalesComponent,
    SaleDetailsComponent
  ],
  imports: [
    CommonModule,
    SalesRoutingModule
  ]
})
export class SalesModule { }

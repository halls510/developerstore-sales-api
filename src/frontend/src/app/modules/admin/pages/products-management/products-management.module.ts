import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductsManagementComponent } from './products-management.component';
import { ProductsManagementRoutingModule } from './products-management-routing.module';
import { FormsModule } from '@angular/forms'; // ✅ Importar FormsModule aqui

@NgModule({
  declarations: [ProductsManagementComponent], // ✅ Declaração correta
  imports: [
    CommonModule,
    ProductsManagementRoutingModule,
    FormsModule
  ]
})
export class ProductsManagementModule { }
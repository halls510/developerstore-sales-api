import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router'; 
import { SharedModule } from '../../shared/shared.module'; // Caminho para o SharedModule
import { FormsModule } from '@angular/forms'; // 👈 importe necessário

import { ProductsRoutingModule } from './products-routing.module';
import { ProductsComponent } from './products.component';
import { ProductsListComponent } from './products-list/products-list.component';


@NgModule({
  declarations: [
    ProductsComponent,
    ProductsListComponent
  ],
  imports: [
    CommonModule,
    ProductsRoutingModule,
    FormsModule, // 👈 adicione aqui
    RouterModule,
    SharedModule  // Agora o SharedModule é importado
  ],
  exports: [
    ProductsListComponent // Exportando para uso em outros módulos
  ]
})
export class ProductsModule { }

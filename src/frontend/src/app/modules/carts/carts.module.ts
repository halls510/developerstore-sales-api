import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CartsRoutingModule } from './carts-routing.module';
import { CartsComponent } from './carts.component';
import { CartsListComponent } from './carts-list/carts-list.component';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    CartsComponent,
    CartsListComponent
  ],
  imports: [
    CommonModule,
    CartsRoutingModule,
    FormsModule
  ]
})
export class CartsModule { }

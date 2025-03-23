import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { ProductsModule } from '../products/products.module';

@NgModule({
  declarations: [
    HomeComponent    
  ],
  imports: [
    CommonModule,
    ProductsModule
  ],
  exports: [HomeComponent]
})
export class HomeModule { }

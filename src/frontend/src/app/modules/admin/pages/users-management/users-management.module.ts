import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';


import { UsersManagementRoutingModule } from './users-management-routing.module';
import { UsersManagementComponent } from './users-management.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms'; // ✅ IMPORTAR AQUI


@NgModule({
  declarations: [
    UsersManagementComponent
  ],
  imports: [
    CommonModule,
    UsersManagementRoutingModule,
    FormsModule,
    ReactiveFormsModule  
  ]
})
export class UsersManagementModule { }
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './modules/login/login.component';
import { HomeComponent } from './modules/home/home.component';
import { authGuard } from './guards/auth.guard';

const routes: Routes = [
  // única rota aberta
  { path: 'login', component: LoginComponent },

  // tudo abaixo protegido com AuthGuard
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'cart',
    loadChildren: () => import('./modules/carts/carts.module').then(m => m.CartsModule),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    loadChildren: () => import('./modules/admin/admin.module').then(m => m.AdminModule),
    canActivate: [authGuard]
  },
  {
    path: 'products',
    loadChildren: () => import('./modules/products/products.module').then(m => m.ProductsModule),
    canActivate: [authGuard]
  },
  {
    path: 'sales',
    loadChildren: () => import('./modules/sales/sales.module').then(m => m.SalesModule),
    canActivate: [authGuard]
  },

  // Rota padrão
  {
    path: '**',
    redirectTo: 'home'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}

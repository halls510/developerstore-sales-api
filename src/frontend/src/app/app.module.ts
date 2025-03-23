import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MockApiInterceptor } from './interceptors/mock-api.service';
import { environment } from '../environments/environment'; // ✅ Corrigida a importação do environment
import { AdminModule } from './modules/admin/admin.module';
import { ProductsManagementModule } from './modules/admin/pages/products-management/products-management.module';
import { LoginModule } from './modules/login/login.module';
import { HomeModule } from './modules/home/home.module';
import { AuthService } from './services/auth.service';
import { AuthInterceptor } from './interceptors/auth.interceptor';

@NgModule({
  declarations: [
    AppComponent       
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    AdminModule,
    ProductsManagementModule,
    FormsModule,
    HomeModule,
    LoginModule 
  ],
  providers: [
    AuthService, // ✅ Certifique-se de que o AuthService está disponível globalmente

    // ✅ O MockApiInterceptor só será usado se o ambiente estiver configurado para mock
    ...(environment.useMockApi ? [{ provide: HTTP_INTERCEPTORS, useClass: MockApiInterceptor, multi: true }] : []),

    // ✅ O AuthInterceptor SEMPRE será utilizado
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
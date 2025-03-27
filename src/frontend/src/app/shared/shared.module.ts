import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { MockApiInterceptor } from '../interceptors/mock-api.service'; // Caminho correto para o interceptor
import { environment } from '../../environments/environment'; // Caminho correto


@NgModule({
  providers: [
    ...(environment.useMockApi ? [{ provide: HTTP_INTERCEPTORS, useClass: MockApiInterceptor, multi: true }] : [])
  ]
})
export class SharedModule { }

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoginResponse } from 'src/app/models/login.model'; 


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private debugMode = false;

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.apiUrl, { email, password }).pipe(
      tap(response => {
        console.log('response login', response);

        if (response.success && response.data?.token) {
          localStorage.setItem('token', response.data.token);
          localStorage.setItem('userRole', response.data.role);   // 👤 guarda role
          localStorage.setItem('userName', response.data.name);   // 👤 guarda nome
          localStorage.setItem('userEmail', response.data.email); // 👤 guarda email
        }
      })
    );
  }

  logout(): void {
    localStorage.clear();
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return this.debugMode || !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserRole(): string | null {
    return localStorage.getItem('userRole');
  }

  getUserId(): number | null {
    const token = this.getToken();
    if (!token) return null;
  
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return Number(payload.nameid); // 👈 é esse campo mesmo
    } catch (err) {
      console.error('Erro ao decodificar token JWT:', err);
      return null;
    }
  }
  
  getCurrentUser(): { id: number, name: string, role: string, email: string } | null {
    const token = this.getToken();
    if (!token) return null;
  
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return {
        id: Number(payload.nameid),
        name: payload.unique_name || localStorage.getItem('userName') || '',
        email: payload.email || localStorage.getItem('userEmail') || '',
        role: payload.role || localStorage.getItem('userRole') || ''
      };
    } catch (err) {
      console.error('Erro ao decodificar token JWT:', err);
      return null;
    }
  } 
  
}

import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'fr';

  constructor(public authService: AuthService, private router: Router){}

  logout(): void {
    this.authService.logout();
  }
  
  isAdmin(): boolean {
    const role = localStorage.getItem('userRole');
    return role === 'Admin' || role === 'Manager';
  }
}

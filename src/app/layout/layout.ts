import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../core/services/auth';

@Component({
selector: 'app-layout',
standalone: true,
imports: [CommonModule, RouterModule],
templateUrl: './layout.html',
styleUrls: ['./layout.css']
})
export class LayoutComponent {
constructor(public authService: AuthService, private router: Router) {}

get username(): string | null {
// Example: decode JWT token and extract username if needed
const token = this.authService.getToken();
if (!token) return null;

try {
  const payload = JSON.parse(atob(token.split('.')[1]));
  return payload?.unique_name || payload?.username || null;
} catch {
  return null;
}

}

logout() {
this.authService.logout();
this.router.navigate(['/login']);
}
}
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
unique_name?: string;
role?: string | string[];
[key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:7164/api/Auth';

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(
      `${this.apiUrl}/login`,
      { username, password }
    );
  }

  saveToken(token: string) {
    localStorage.setItem('jwtToken', token);
  }

  getToken(): string | null {
    return localStorage.getItem('jwtToken');
  }

  logout() {
  localStorage.removeItem('jwtToken');
  window.location.href = '/login'; // Full reload to reset app state
  }

  getUserRole(): string | null {
  const token = this.getToken();
  if (!token) return null;
  try {
  const payload: JwtPayload = jwtDecode(token);
  if (Array.isArray(payload.role)) {
  return payload.role[0]; // Return first role
  }
  return payload.role || null;
  } catch {
  return null;
  }
  }

  isAdmin(): boolean {
  return this.getUserRole() === 'Admin';
  }

  isEmployee(): boolean {
  return this.getUserRole() === 'Employee';
  }

 isAuthenticated(): boolean {
const token = this.getToken();
return !!token; // returns true if token exists, false otherwise
}
}


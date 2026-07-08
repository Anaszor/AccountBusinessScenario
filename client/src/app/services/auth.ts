import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

export interface UserInfo {
  email: string;
  roles: string[];
}

export interface LoginResponse {
  token: string;
  expiration: string;
  user: UserInfo;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5041/api/auth';

  readonly token = signal<string | null>(localStorage.getItem('token'));
  readonly currentUser = signal<UserInfo | null>(JSON.parse(localStorage.getItem('user') || 'null'));

  readonly isAuthenticated = computed(() => !!this.token());
  readonly isAdmin = computed(() => this.currentUser()?.roles.includes('Admin') ?? false);

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: { email: string; password: string }): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        localStorage.setItem('user', JSON.stringify(res.user));
        this.token.set(res.token);
        this.currentUser.set(res.user);
      })
    );
  }

  register(userData: { email: string; password: string; role?: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }

  getUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/users`);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.token.set(null);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }
}

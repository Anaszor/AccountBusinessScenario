import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AccountStatement {
  id: string;
  customerId: string;
  month: string;
  balance: number;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class StatementService {
  private readonly apiUrl = 'http://localhost:5041/api/statements';

  constructor(private http: HttpClient) {}

  getStatements(customerId: string, month: string): Observable<AccountStatement[]> {
    return this.http.get<AccountStatement[]>(`${this.apiUrl}?customerId=${encodeURIComponent(customerId)}&month=${encodeURIComponent(month)}`);
  }

  getMyStatements(month: string): Observable<AccountStatement[]> {
    return this.http.get<AccountStatement[]>(`${this.apiUrl}/my-statements?month=${encodeURIComponent(month)}`);
  }

  generateStatements(month: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/generate`, { month });
  }
}

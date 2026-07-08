import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Customer {
  id?: number;
  name: string;
  gender: string;
  dateOfBirth: string;
  email: string;
  phone: string;
  photo?: string | null;
  address: string;
  balance: number;
  custmerType?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly apiUrl = 'http://localhost:5041/api/BusinessCustomer';

  constructor(private http: HttpClient) {}

  getAll(search?: string): Observable<Customer[]> {
    const url = search ? `${this.apiUrl}?search=${encodeURIComponent(search)}` : this.apiUrl;
    return this.http.get<Customer[]>(url);
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`);
  }

  create(customer: Customer): Observable<Customer> {
    return this.http.post<Customer>(this.apiUrl, customer);
  }

  update(id: number, customer: Customer): Observable<Customer> {
    return this.http.put<Customer>(`${this.apiUrl}/${id}`, customer);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

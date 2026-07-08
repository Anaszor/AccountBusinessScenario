import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  isLoginMode = signal(true);
  email = '';
  password = '';
  confirmPassword = '';
  role = 'User';
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  isLoading = signal(false);

  constructor(private authService: AuthService, private router: Router) {}

  setMode(isLogin: boolean): void {
    if (this.isLoginMode() === isLogin) return;
    this.isLoginMode.set(isLogin);
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.email = '';
    this.password = '';
    this.confirmPassword = '';
  }

  onSubmit(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (!this.email || !this.password) {
      this.errorMessage.set('Please fill out all required fields.');
      return;
    }

    if (!this.isLoginMode() && this.password !== this.confirmPassword) {
      this.errorMessage.set('Passwords do not match.');
      return;
    }

    this.isLoading.set(true);

    if (this.isLoginMode()) {
      this.authService.login({ email: this.email, password: this.password }).subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res.user.roles.includes('Admin')) {
            this.router.navigate(['/admin-dashboard']);
          } else {
            this.router.navigate(['/user-dashboard']);
          }
        },
        error: (err) => {
          this.isLoading.set(false);
          this.errorMessage.set(err.error?.message || 'Login failed. Please check your credentials.');
        }
      });
    } else {
      this.authService.register({ email: this.email, password: this.password, role: this.role }).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.successMessage.set('Account created successfully! Please log in.');
          this.isLoginMode.set(true);
          this.password = '';
          this.confirmPassword = '';
        },
        error: (err) => {
          this.isLoading.set(false);
          let msg = 'Registration failed.';
          
          if (err.error) {
            if (err.error.message) {
              msg += ' ' + err.error.message;
            } else if (err.error.Message) {
              msg += ' ' + err.error.Message;
            }
            
            if (err.error.errors && Array.isArray(err.error.errors)) {
              msg += ' ' + err.error.errors.map((e: any) => e.description || e).join(' ');
            } else if (err.error.errors && typeof err.error.errors === 'object') {
              const errorObj = err.error.errors;
              const messages: string[] = [];
              for (const key in errorObj) {
                if (Object.prototype.hasOwnProperty.call(errorObj, key)) {
                  const errList = errorObj[key];
                  if (Array.isArray(errList)) {
                    messages.push(`${key}: ${errList.join(', ')}`);
                  } else {
                    messages.push(`${key}: ${errList}`);
                  }
                }
              }
              if (messages.length > 0) {
                msg += ' ' + messages.join(' ');
              }
            }
          } else {
            msg += ' Please verify your inputs and connection.';
          }
          
          this.errorMessage.set(msg);
        }
      });
    }
  }
}

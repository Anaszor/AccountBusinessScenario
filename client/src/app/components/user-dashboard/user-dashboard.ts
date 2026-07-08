import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountStatement, StatementService } from '../../services/statement';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css'
})
export class UserDashboardComponent implements OnInit {
  searchMonth = '';
  statements = signal<AccountStatement[]>([]);
  
  isLoading = signal(false);
  hasSearched = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(
    protected authService: AuthService,
    private statementService: StatementService
  ) {}

  ngOnInit(): void {
    // Default to April 2026 format or current month
    this.searchMonth = '2026-04'; // Default placeholder matching standard dataset
    this.onSearch();
  }

  onSearch(): void {
    this.errorMessage.set(null);
    this.hasSearched.set(true);

    if (!this.searchMonth) {
      this.errorMessage.set('Please select a Month.');
      this.statements.set([]);
      return;
    }

    const formattedMonth = this.formatMonthString(this.searchMonth);

    this.isLoading.set(true);
    this.statementService.getMyStatements(formattedMonth).subscribe({
      next: (data) => {
        this.isLoading.set(false);
        this.statements.set(data);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to load statements. Check if your account is linked to a customer.');
        this.statements.set([]);
      }
    });
  }

  formatMonthString(input: string): string {
    if (!input || !input.includes('-')) return input;
    const parts = input.split('-');
    const year = parts[0];
    const monthIndex = parseInt(parts[1], 10) - 1;
    const monthNames = [
      'January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December'
    ];
    return `${monthNames[monthIndex]} ${year}`;
  }
}

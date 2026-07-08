import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Customer, CustomerService } from '../../services/customer';
import { StatementService } from '../../services/statement';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboardComponent implements OnInit {
  customers = signal<Customer[]>([]);
  searchQuery = '';
  
  totalCustomers = signal(0);
  totalBalance = signal(0);
  
  showForm = signal(false);
  isEditMode = signal(false);
  
  formCustomer: Customer = this.getEmptyCustomer();
  
  statementMonth = '';
  isGenerating = signal(false);
  genSuccess = signal<string | null>(null);
  genError = signal<string | null>(null);
  
  successAlert = signal<string | null>(null);
  errorAlert = signal<string | null>(null);

  // Tabs management
  activeTab = signal<'customers' | 'users'>('customers');
  usersList = signal<any[]>([]);

  constructor(
    private customerService: CustomerService,
    private statementService: StatementService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  setTab(tab: 'customers' | 'users'): void {
    this.activeTab.set(tab);
    if (tab === 'users') {
      this.loadUsers();
    } else {
      this.loadCustomers();
    }
  }

  loadUsers(): void {
    this.authService.getUsers().subscribe({
      next: (data) => this.usersList.set(data),
      error: () => this.errorAlert.set('Failed to load registered users.')
    });
  }

  loadCustomers(): void {
    this.customerService.getAll(this.searchQuery).subscribe({
      next: (data) => {
        this.customers.set(data);
        this.totalCustomers.set(data.length);
        this.totalBalance.set(data.reduce((sum, c) => sum + (c.balance || 0), 0));
      },
      error: () => this.errorAlert.set('Failed to load customers.')
    });
  }

  onSearch(): void {
    this.loadCustomers();
  }

  getEmptyCustomer(): Customer {
    return {
      name: '',
      gender: 'Male',
      dateOfBirth: '',
      email: '',
      phone: '',
      photo: '',
      address: '',
      balance: 0,
      custmerType: 'Credit Card'
    };
  }

  openAddModal(): void {
    this.formCustomer = this.getEmptyCustomer();
    this.isEditMode.set(false);
    this.showForm.set(true);
    this.successAlert.set(null);
    this.errorAlert.set(null);
  }

  openEditModal(customer: Customer): void {
    this.formCustomer = { ...customer };
    this.isEditMode.set(true);
    this.showForm.set(true);
    this.successAlert.set(null);
    this.errorAlert.set(null);
  }

  closeModal(): void {
    this.showForm.set(false);
  }

  onSubmitCustomer(): void {
    this.successAlert.set(null);
    this.errorAlert.set(null);

    if (this.isEditMode()) {
      if (this.formCustomer.id === undefined) return;
      this.customerService.update(this.formCustomer.id, this.formCustomer).subscribe({
        next: () => {
          this.successAlert.set('Customer updated successfully.');
          this.loadCustomers();
          this.closeModal();
        },
        error: (err) => this.errorAlert.set(err.error?.message || 'Failed to update customer.')
      });
    } else {
      this.customerService.create(this.formCustomer).subscribe({
        next: () => {
          this.successAlert.set('Customer created successfully.');
          this.loadCustomers();
          this.closeModal();
        },
        error: (err) => this.errorAlert.set(err.error?.message || 'Failed to create customer.')
      });
    }
  }

  onDeleteCustomer(id: number | undefined): void {
    if (id === undefined) return;
    if (confirm('Are you sure you want to delete this customer? This action is permanent.')) {
      this.customerService.delete(id).subscribe({
        next: () => {
          this.successAlert.set('Customer deleted successfully.');
          this.loadCustomers();
        },
        error: () => this.errorAlert.set('Failed to delete customer.')
      });
    }
  }

  onGenerateStatements(): void {
    this.genSuccess.set(null);
    this.genError.set(null);

    if (!this.statementMonth) {
      this.genError.set('Please select a month.');
      return;
    }

    const formattedMonth = this.formatMonthString(this.statementMonth);
    this.isGenerating.set(true);
    
    this.statementService.generateStatements(formattedMonth).subscribe({
      next: () => {
        this.isGenerating.set(false);
        this.genSuccess.set(`Account statements for ${formattedMonth} generated and credit card statements emailed successfully!`);
        this.statementMonth = '';
        this.loadCustomers();
      },
      error: (err) => {
        this.isGenerating.set(false);
        this.genError.set(err.error?.message || 'Failed to generate statements. Check SMTP credentials in appsettings.json.');
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

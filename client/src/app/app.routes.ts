import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { ShellComponent } from './components/shell/shell';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard';
import { UserDashboardComponent } from './components/user-dashboard/user-dashboard';
import { authGuard, adminGuard } from './guards/auth';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: 'admin-dashboard', component: AdminDashboardComponent, canActivate: [adminGuard] },
      { path: 'user-dashboard', component: UserDashboardComponent },
      { path: '', redirectTo: 'user-dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];

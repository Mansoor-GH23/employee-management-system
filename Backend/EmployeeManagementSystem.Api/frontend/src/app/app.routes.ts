import { Routes } from '@angular/router';
import { provideRouter } from '@angular/router';
import { LoginComponent } from './features/auth/login';
import { LayoutComponent } from './layout/layout';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
{
path: '',
redirectTo: 'login',
pathMatch: 'full'
},
{
path: 'login',
loadChildren: () =>
import('./features/auth/login.routes').then((m) => m.routes)
},
{
path: '',
component: LayoutComponent,
canActivate: [authGuard],
children: [
{
path: 'employee',
loadChildren: () =>
import('./features/employee/employee.routes').then((m) => m.routes)
}
]
},
{
path: '**',
redirectTo: 'login'
}
];
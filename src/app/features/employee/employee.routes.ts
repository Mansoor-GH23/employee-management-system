import { Routes } from '@angular/router';
import { EmployeeListComponent } from './employee-list';
import { EmployeeFormComponent } from './employee-form';
import { authGuard } from '../../core/guards/auth-guard';

export const routes: Routes = [
{
path: '',
component: EmployeeListComponent,
canActivate: [authGuard]
},
{
path: 'add',
component: EmployeeFormComponent,
canActivate: [authGuard]
},
{
path: 'edit/:employeeCode',
component: EmployeeFormComponent,
canActivate: [authGuard]
}
];
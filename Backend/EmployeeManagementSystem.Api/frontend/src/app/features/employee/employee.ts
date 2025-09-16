import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth';
import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeModel } from '../../core/models/employee.model';

@Component({
selector: 'app-employee',
standalone: true,
imports: [CommonModule],
templateUrl: './employee.html',
styleUrls: ['./employee.css']
})
export class Employee implements OnInit {
employees: EmployeeModel[] = [];
isLoading = true;
error: string | null = null;

constructor(
private authService: AuthService,
private employeeService: EmployeeService,
private router: Router
) {}

ngOnInit(): void {
this.employeeService.getAllEmployees().subscribe({
next: (data) => {
this.employees = data;
this.isLoading = false;
},
error: (err) => {
this.error = 'Failed to load employees';
this.isLoading = false;
}
});
}

logout() {
this.authService.logout();
this.router.navigate(['/login']);
}
}
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeModel } from '../../core/models/employee.model';

@Component({
selector: 'app-employee-list',
standalone: true,
imports: [CommonModule],
templateUrl: './employee-list.html',
styleUrls: ['./employee-list.css']
})
export class EmployeeListComponent implements OnInit {
employees: EmployeeModel[] = [];
isLoading = true;
error: string | null = null;

constructor(
private router: Router,
private employeeService: EmployeeService
) {}

ngOnInit(): void {
this.loadEmployees();
}

loadEmployees(): void {
this.isLoading = true;
this.employeeService.getAllEmployees().subscribe({
next: (res) => {
this.employees = res;
this.isLoading = false;
},
error: () => {
this.error = 'Failed to load employees';
this.isLoading = false;
}
});
}

addEmployee(): void {
this.router.navigate(['employee/add']);
}

editEmployee(id: string): void {
this.router.navigate(['employee/edit', id]);
}

deleteEmployee(id: string): void {
const confirmDelete = confirm('Are you sure you want to delete this employee?');
if (confirmDelete) {
this.employeeService.deleteEmployee(id).subscribe({
next: () => this.loadEmployees(),
error: () => alert('Delete failed')
});
}
}
}
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeModel } from '../../core/models/employee.model';
import { exitCode } from 'process';

@Component({
selector: 'app-employee-form',
standalone: true,
imports: [CommonModule, FormsModule],
templateUrl: './employee-form.html',
styleUrls: ['./employee-form.css']
})
export class EmployeeFormComponent implements OnInit {
employee: EmployeeModel = {
id: 0,
employeeCode: '',
fullName: '',
email: '',
department: '',
dateOfJoining: new Date(),
salary: 0
};

isEditMode = false;
isSubmitting = false;
error: string | null = null;

constructor(
private employeeService: EmployeeService,
private route: ActivatedRoute,
private router: Router
) {}

ngOnInit(): void {
const id = this.route.snapshot.paramMap.get('employeeCode');
if (id) {
this.isEditMode = true;
this.employeeService.getEmployee(id).subscribe({
next: (data) => {
this.employee = data;
},
error: () => {
this.error = 'Failed to load employee';
}
});
}
}

onSubmit(): void {
this.isSubmitting = true;
const action$ = this.isEditMode
? this.employeeService.updateEmployee(this.employee)
: this.employeeService.addEmployee(this.employee);

action$.subscribe({
  next: () => {
    alert(this.isEditMode ? 'Employee updated successfully!' : 'Employee added successfully!');
    this.router.navigate(['/employee']);
  },
  error: () => {
    this.error = this.isEditMode ? 'Update failed' : 'Creation failed';
    this.isSubmitting = false;
  }
});

}

cancel(): void {
this.router.navigate(['/employee']);
}
}
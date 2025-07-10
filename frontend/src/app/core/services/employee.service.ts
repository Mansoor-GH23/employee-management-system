import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeModel } from '../../core/models/employee.model'; // Adjust path if needed

@Injectable({
providedIn: 'root'
})
export class EmployeeService {
private apiUrl = 'https://localhost:7164/api/Employee';

constructor(private http: HttpClient) {}

getAllEmployees(): Observable<EmployeeModel[]> {
return this.http.get<EmployeeModel[]>(this.apiUrl, {
responseType: 'json' as const
});
}

// Get single employee by ID
getEmployee(id: string): Observable<EmployeeModel> {
  return this.http.get<EmployeeModel>(`${this.apiUrl}/${id}`);
}

// Add new employee
addEmployee(employee: EmployeeModel): Observable<EmployeeModel> { 
return this.http.post<EmployeeModel>(this.apiUrl, employee, {
responseType: 'json' as const
});
}

// Update employee
updateEmployee(employee: EmployeeModel): Observable<EmployeeModel> {
return this.http.put<EmployeeModel>(`${this.apiUrl}/${employee.employeeCode}`, employee);
}

// Delete employee
deleteEmployee(id: string): Observable<void> {
  return this.http.delete<void>(`${this.apiUrl}/${id}`);
}
}
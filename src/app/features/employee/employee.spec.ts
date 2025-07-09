import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Employee } from './employee';
import { AuthService } from '../../core/services/auth';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('Employee', () => {
  let component: Employee;
  let fixture: ComponentFixture<Employee>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Employee],
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Employee);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth';

describe('AuthService', () => {
let service: AuthService;
let httpMock: HttpTestingController;

const mockToken = 'dummy.jwt.token';
const mockResponse = { token: mockToken };

beforeEach(() => {
TestBed.configureTestingModule({
imports: [HttpClientTestingModule],
providers: [AuthService]
});

service = TestBed.inject(AuthService);
httpMock = TestBed.inject(HttpTestingController);
localStorage.clear(); // reset token storage

});

afterEach(() => {
httpMock.verify(); // ensure no outstanding requests
localStorage.clear();
});

it('should be created', () => {
expect(service).toBeTruthy();
});

it('should store token after login', () => {
service.login('admin', 'Admin123').subscribe(res => {
expect(res.token).toEqual(mockToken);
expect(localStorage.getItem('jwtToken')).toEqual(mockToken);
});

const req = httpMock.expectOne('https://localhost:7164/api/Auth/login');
expect(req.request.method).toBe('POST');
expect(req.request.body).toEqual({ username: 'admin', password: 'Admin123' });

req.flush(mockResponse); // simulate backend response

});

it('should return token from localStorage', () => {
localStorage.setItem('jwtToken', mockToken);
const token = service.getToken();
expect(token).toBe(mockToken);
});

it('should remove token on logout', () => {
localStorage.setItem('jwtToken', mockToken);
spyOn(window.location, 'assign');
service.logout();
expect(localStorage.getItem('jwtToken')).toBeNull();
expect(window.location.assign).toHaveBeenCalledWith('/login');
});

it('should return true if authenticated (token exists)', () => {
localStorage.setItem('jwtToken', mockToken);
expect(service.isAuthenticated()).toBeTrue();
});

it('should return false if not authenticated (no token)', () => {
expect(service.isAuthenticated()).toBeFalse();
});
});
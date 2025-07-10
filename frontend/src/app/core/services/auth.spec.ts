import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

class MockRouter {
  navigate = jasmine.createSpy('navigate').and.returnValue(Promise.resolve(true));
}

class MockLocalStorage {
  private store: { [key: string]: string } = {};

  getItem(key: string): string | null {
    return this.store[key] || null;
  }

  setItem(key: string, value: string) {
    this.store[key] = value;
  }

  removeItem(key: string) {
    delete this.store[key];
  }

  clear() {
    this.store = {};
  }
}

describe('AuthService', () => {
  let service: AuthService;
  let router: Router;
  let mockStorage: MockLocalStorage;

  beforeEach(() => {
    mockStorage = new MockLocalStorage();

    spyOn(window.localStorage, 'getItem').and.callFake((key: string) =>
      mockStorage.getItem(key)
    );
    spyOn(window.localStorage, 'setItem').and.callFake((key: string, value: string) =>
      mockStorage.setItem(key, value)
    );
    spyOn(window.localStorage, 'removeItem').and.callFake((key: string) =>
      mockStorage.removeItem(key)
    );

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(withInterceptors([])),
        { provide: Router, useClass: MockRouter }
      ]
    });

    service = TestBed.inject(AuthService);
    router = TestBed.inject(Router);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return false if no token exists', () => {
    expect(service.isAuthenticated()).toBeFalse();
  });

  it('should return true if token exists and not expired', () => {
    const token = 'valid.token.here';
    const futureExp = Math.floor(Date.now() / 1000) + 3600;
    mockStorage.setItem('jwtToken', token);

    spyOn<any>(service, 'decodeToken').and.returnValue({ exp: futureExp });

    expect(service.isAuthenticated()).toBeTrue();
  });

  it('should return false if token is expired', () => {
    const token = 'expired.token.here';
    const pastExp = Math.floor(Date.now() / 1000) - 3600;
    mockStorage.setItem('jwtToken', token);

    spyOn<any>(service, 'decodeToken').and.returnValue({ exp: pastExp });

    expect(service.isAuthenticated()).toBeFalse();
  });

  it('should retrieve token correctly', () => {
    const token = 'sample.jwt.token';
    mockStorage.setItem('jwtToken', token);
    expect(service.getToken()).toBe(token);
  });

  it('should remove token and navigate to login on logout', async () => {
  mockStorage.setItem('jwtToken', 'any.token');
  await service.logout();
  expect(localStorage.removeItem).toHaveBeenCalledWith('jwtToken');
  expect(router.navigate).toHaveBeenCalledWith(['/login']);
});

});

import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/auth-interceptor';

export const appProviders = [
  provideHttpClient(withInterceptorsFromDi()),
  importProvidersFrom(FormsModule),
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
];

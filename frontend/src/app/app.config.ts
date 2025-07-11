import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { routes } from './app.routes';
import { appProviders } from './app.providers';

export const appConfig: ApplicationConfig = {
providers: [
provideHttpClient(), // ✅ Fix added here
provideRouter(routes),
...appProviders,
],
};
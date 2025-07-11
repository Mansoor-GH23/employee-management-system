import 'zone.js'; // 👈 Add this line at the very top
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, appConfig)
.catch(err => console.error(err));
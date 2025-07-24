import 'zone.js'; // 👈 Add this line at the very top
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, appConfig)
.catch(err => console.error(err));

// Test CI workflow in main.ts ...434334343cd.daada
// Test CD workflow in main.ts ...
// Test Main branch to test GitHub Actions workflow for CD
// Testing CD workflow in main.ts after changes in frontend-cd.yml.
// Testing frontend-cd.yml for CD workflow in main.ts...
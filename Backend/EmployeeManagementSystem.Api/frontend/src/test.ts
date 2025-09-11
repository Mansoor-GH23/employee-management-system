import 'zone.js';  // required for Angular testing
import 'zone.js/testing';

import { getTestBed } from '@angular/core/testing';
import {
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting
} from '@angular/platform-browser-dynamic/testing';

// Initialize testing environment
getTestBed().initTestEnvironment(
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting()
);

// Manually import your spec files
import './app/core/services/auth.spec';
import './app/core/guards/auth-guard.spec';
import './app/features/employee/employee-list.spec';
// Add more .spec.ts imports here as needed

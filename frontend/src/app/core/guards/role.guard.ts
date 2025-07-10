import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const adminGuard: CanActivateFn = () => {
const authService = inject(AuthService);
const router = inject(Router);
if (authService.isAdmin()) {
return true;
}
alert('Access denied. Admins only.');
router.navigate(['/login']);
return false;
};
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth';

//For git changes to record
//for git changes to record new
@Component({
selector: 'app-login',
standalone: true,
imports: [CommonModule, FormsModule],
templateUrl: './login.html',
styleUrls: ['./login.css'],
})
export class LoginComponent {
username = '';
password = '';

constructor(private authService: AuthService, private router: Router) {}

onSubmit() {
this.authService.login(this.username, this.password).subscribe({
next: (res) => {
this.authService.saveToken(res.token);
alert('Login successful!');
// Optionally: this.router.navigate(['/employee']);
},
error: () => {
alert('Login failed');
},
});
}

ngOnInit() {
  if (this.authService.getToken()) {
    this.router.navigate(['/employee']); // or any default private page
  }
    }

}
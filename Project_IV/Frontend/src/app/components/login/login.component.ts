import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  form: FormGroup;
  loading = false;
  errorMsg = '';
  showPassword = false;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    if (this.auth.isLoggedIn()) {
      this.router.navigate([this.auth.getRole() === 'Admin' ? '/admin/dashboard' : '/events']);
    }
    this.form = this.fb.group({
      emailId:  ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  f(name: string) { return this.form.get(name); }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.errorMsg = '';
    this.auth.login(this.form.value).subscribe({
      next: () => {
        const role = this.auth.getRole();
        this.router.navigate([role === 'Admin' ? '/admin/dashboard' : '/events']);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Login failed. Check your credentials.';
        this.loading = false;
      }
    });
  }
}

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
<div class="row justify-content-center mt-5">
  <div class="col-md-4">
    <div class="card shadow">
      <div class="card-header bg-dark text-white text-center">
        <h5 class="mb-0">🔐 Admin Login</h5>
        <small class="opacity-75">upGrad Virtual Event Management System</small>
      </div>
      <div class="card-body">
        <div *ngIf="errorMsg" class="alert alert-danger py-2">{{ errorMsg }}</div>
        <form [formGroup]="form" (ngSubmit)="onSubmit()" novalidate>
          <div class="mb-3">
            <label class="form-label fw-semibold">Admin Email <span class="text-danger">*</span></label>
            <input type="email" class="form-control" formControlName="emailId"
                   placeholder="admin@ems.com"
                   [class.is-invalid]="f('emailId')?.invalid && f('emailId')?.touched">
            <div class="invalid-feedback">Enter a valid email.</div>
          </div>
          <div class="mb-3">
            <label class="form-label fw-semibold">Password <span class="text-danger">*</span></label>
            <input type="password" class="form-control" formControlName="password"
                   placeholder="Password"
                   [class.is-invalid]="f('password')?.invalid && f('password')?.touched">
            <div class="invalid-feedback">Password is required.</div>
          </div>
          <button type="submit" class="btn btn-dark w-100 fw-bold" [disabled]="loading">
            <span *ngIf="loading" class="spinner-border spinner-border-sm me-1"></span>
            Login as Admin
          </button>
        </form>
        <p class="mt-3 text-center text-muted small">
          Default: <code>admin&#64;ems.com</code> / <code>Admin&#64;123</code>
        </p>
      </div>
    </div>
    <div class="text-center mt-3">
      <a routerLink="/events" class="text-muted small">← Back to Public Site</a>
    </div>
  </div>
</div>
  `
})
export class AdminLoginComponent {
  form: FormGroup;
  loading = false;
  errorMsg = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    if (this.auth.getRole() === 'Admin') this.router.navigate(['/admin/dashboard']);
    this.form = this.fb.group({
      emailId:  ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  f(n: string) { return this.form.get(n); }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.auth.login(this.form.value).subscribe({
      next: () => {
        if (this.auth.getRole() === 'Admin') this.router.navigate(['/admin/dashboard']);
        else { this.auth.logout(); this.errorMsg = 'Not an admin account.'; this.loading = false; }
      },
      error: (err) => { this.errorMsg = err.error?.message ?? 'Login failed.'; this.loading = false; }
    });
  }
}

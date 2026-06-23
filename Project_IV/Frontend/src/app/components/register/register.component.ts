import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

function passwordStrength(control: AbstractControl): ValidationErrors | null {
  const val: string = control.value ?? '';
  if (!/[A-Z]/.test(val) || !/[^a-zA-Z0-9]/.test(val)) return { passwordStrength: true };
  return null;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  form: FormGroup;
  loading = false;
  errorMsg = '';
  successMsg = '';
  showPassword = false;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      emailId:  ['', [Validators.required, Validators.email]],
      userName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(50)]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(20), passwordStrength]]
    });
  }

  f(name: string) { return this.form.get(name); }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.errorMsg = '';
    this.successMsg = '';
    this.auth.register(this.form.value).subscribe({
      next: (res) => {
        this.successMsg = res.message ?? 'Registration successful!';
        this.loading = false;
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Registration failed.';
        this.loading = false;
      }
    });
  }
}

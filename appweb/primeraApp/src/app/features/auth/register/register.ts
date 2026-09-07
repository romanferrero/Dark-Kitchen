import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../core/services/auth';
import { AlertBanner } from '../../../shared/components/alert-banner/alert-banner';
import { AuthCard } from '../../../shared/components/auth-card/auth-card';
import { TextInput } from '../../../shared/components/text-input/text-input';
import { Button } from '../../../shared/components/button/button';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink, AlertBanner, AuthCard, TextInput, Button],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private auth = inject(Auth);
  private router = inject(Router);

  loading = signal(false);
  errorMessage = signal<string | null>(null);

  registerForm = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(25)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(15), Validators.maxLength(25)]],
  });

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);
    const { firstName, lastName, email, phone, password } = this.registerForm.getRawValue();

    this.auth
      .register({
        firstName: firstName!,
        lastName: lastName!,
        email: email!,
        phone: phone!,
        password: password!,
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.router.navigate(['/auth/login']);
        },
        error: (err) => {
          this.loading.set(false);
          this.errorMessage.set(
            err.error?.errorMessage ?? 'Could not complete registration. Check entered data.',
          );
        },
      });
  }

  get firstName() {
    return this.registerForm.controls.firstName;
  }
  get lastName() {
    return this.registerForm.controls.lastName;
  }
  get email() {
    return this.registerForm.controls.email;
  }
  get phone() {
    return this.registerForm.controls.phone;
  }
  get password() {
    return this.registerForm.controls.password;
  }
}

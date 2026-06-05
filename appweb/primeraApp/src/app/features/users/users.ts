import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../core/services/user';

@Component({
  selector: 'app-users',
  imports: [ReactiveFormsModule],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);

  loading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  userForm = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(25)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(15), Validators.maxLength(25)]],
    role: ['', [Validators.required]],
  });

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const { firstName, lastName, email, phone, password, role } = this.userForm.getRawValue();

    this.userService
      .create({
        firstName: firstName!,
        lastName: lastName!,
        email: email!,
        phone: phone!,
        password: password!,
        role: role!,
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.userForm.reset();
          this.successMessage.set('Usuario creado correctamente.');
        },
        error: (err) => {
          this.loading.set(false);
          this.errorMessage.set(
            err.error?.message ?? 'No se pudo crear el usuario. Revisá los datos ingresados.',
          );
        },
      });
  }

  get firstName() {
    return this.userForm.controls.firstName;
  }
  get lastName() {
    return this.userForm.controls.lastName;
  }
  get email() {
    return this.userForm.controls.email;
  }
  get phone() {
    return this.userForm.controls.phone;
  }
  get password() {
    return this.userForm.controls.password;
  }
  get role() {
    return this.userForm.controls.role;
  }
}

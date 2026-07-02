import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

function passwordsMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
}

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink],
    templateUrl: './register.html',
    styleUrl: './register.scss'
})
export class Register {
    private readonly fb = inject(FormBuilder);
    private readonly authService = inject(AuthService);
    private readonly router = inject(Router);

    readonly isLoading = signal(false);
    readonly errorMessage = signal<string | null>(null);

    readonly form = this.fb.nonNullable.group(
        {
            displayName: ['', [Validators.required, Validators.minLength(2)]],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required]]
        },
        { validators: passwordsMatchValidator }
    );

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isLoading.set(true);
        this.errorMessage.set(null);

        const { confirmPassword, ...request } = this.form.getRawValue();

        this.authService.register(request).subscribe({
            next: () => {
                this.isLoading.set(false);
                this.router.navigate(['/tasks']);
            },
            error: (err) => {
                this.isLoading.set(false);
                this.errorMessage.set(err?.error?.error ?? 'Не вдалося зареєструватися.');
            }
        });
    }
}
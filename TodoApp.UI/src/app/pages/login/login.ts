import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink],
    templateUrl: './login.html',
    styleUrl: './login.scss'
})
export class Login {
    private readonly fb = inject(FormBuilder);
    private readonly authService = inject(AuthService);
    private readonly router = inject(Router);

    readonly isLoading = signal(false);
    readonly errorMessage = signal<string | null>(null);

    readonly form = this.fb.nonNullable.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]]
    });

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isLoading.set(true);
        this.errorMessage.set(null);

        this.authService.login(this.form.getRawValue()).subscribe({
            next: () => {
                this.isLoading.set(false);
                this.router.navigate(['/tasks']);
            },
            error: () => {
                this.isLoading.set(false);
                this.errorMessage.set('Невірний email або пароль.');
            }
        });
    }
}
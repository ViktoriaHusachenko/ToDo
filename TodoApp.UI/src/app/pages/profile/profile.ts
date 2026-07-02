import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [],
    templateUrl: './profile.html',
    styleUrl: './profile.scss'
})
export class Profile {
    private readonly authService = inject(AuthService);

    readonly currentUser = this.authService.currentUser;

    logout(): void {
        this.authService.logout();
    }
}
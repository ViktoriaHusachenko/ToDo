import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  private readonly authService = inject(AuthService);

  readonly currentUser = this.authService.currentUser;

  logout(): void {
    this.authService.logout();
  }
}
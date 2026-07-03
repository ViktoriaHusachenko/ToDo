import { Component, inject } from '@angular/core';
import { SettingsService, ThemeMode } from '../../core/services/settings.service';

const ACCENT_PALETTE = ['#0078d4', '#d13438', '#ca5010', '#107c10', '#5c2d91', '#e3008c', '#008272', '#605e5c'];

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [],
  templateUrl: './settings.html',
  styleUrl: './settings.scss'
})
export class Settings {
  readonly settingsService = inject(SettingsService);
  readonly accentPalette = ACCENT_PALETTE;

  setTheme(theme: ThemeMode): void {
    this.settingsService.setTheme(theme);
  }

  setAccent(color: string): void {
    this.settingsService.setAccentColor(color);
  }

  toggleSounds(): void {
    this.settingsService.toggleSounds();
  }

  testSound(): void {
    this.settingsService.playCompleteSound();
  }

  toggleNotifications(): void {
    this.settingsService.toggleNotifications();
  }

  testNotification(): void {
    this.settingsService.showTestNotification();
  }
}
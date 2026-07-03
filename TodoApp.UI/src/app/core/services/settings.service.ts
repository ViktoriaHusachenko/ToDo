import { Injectable, effect, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

const THEME_KEY = 'settings_theme';
const ACCENT_KEY = 'settings_accent_color';
const SOUNDS_KEY = 'settings_sounds_enabled';
const NOTIFICATIONS_KEY = 'settings_notifications_enabled';
const DEFAULT_ACCENT = '#0078d4';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  readonly theme = signal<ThemeMode>(this.readTheme());
  readonly accentColor = signal<string>(localStorage.getItem(ACCENT_KEY) ?? DEFAULT_ACCENT);
  readonly soundsEnabled = signal<boolean>(this.readBool(SOUNDS_KEY, true));
  readonly notificationsEnabled = signal<boolean>(this.readBool(NOTIFICATIONS_KEY, false));

  constructor() {
    effect(() => {
      const theme = this.theme();
      document.documentElement.setAttribute('data-bs-theme', theme);
      localStorage.setItem(THEME_KEY, theme);
    });

    effect(() => {
      const color = this.accentColor();
      document.documentElement.style.setProperty('--accent-color', color);
      localStorage.setItem(ACCENT_KEY, color);
    });

    effect(() => localStorage.setItem(SOUNDS_KEY, String(this.soundsEnabled())));
    effect(() => localStorage.setItem(NOTIFICATIONS_KEY, String(this.notificationsEnabled())));
  }

  setTheme(theme: ThemeMode): void {
    this.theme.set(theme);
  }

  setAccentColor(color: string): void {
    this.accentColor.set(color);
  }

  toggleSounds(): void {
    this.soundsEnabled.set(!this.soundsEnabled());
  }

  async toggleNotifications(): Promise<void> {
    if (!this.notificationsEnabled()) {
      if (!('Notification' in window)) {
        alert('Цей браузер не підтримує сповіщення.');
        return;
      }
      const permission = await Notification.requestPermission();
      if (permission !== 'granted') {
        alert('Дозвіл на сповіщення не надано.');
        return;
      }
    }
    this.notificationsEnabled.set(!this.notificationsEnabled());
  }

  showTestNotification(): void {
    if (this.notificationsEnabled() && Notification.permission === 'granted') {
      new Notification('TodoApp', { body: 'Це тестове сповіщення.' });
    }
  }

  playCompleteSound(): void {
    if (!this.soundsEnabled()) return;

    try {
      const AudioContextClass = window.AudioContext || (window as any).webkitAudioContext;
      const ctx = new AudioContextClass();
      const oscillator = ctx.createOscillator();
      const gain = ctx.createGain();

      oscillator.type = 'sine';
      oscillator.frequency.setValueAtTime(880, ctx.currentTime);
      gain.gain.setValueAtTime(0.15, ctx.currentTime);
      gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.2);

      oscillator.connect(gain);
      gain.connect(ctx.destination);
      oscillator.start();
      oscillator.stop(ctx.currentTime + 0.2);
    } catch {
      // AudioContext недоступний — тихо ігноруємо
    }
  }

  private readTheme(): ThemeMode {
    return localStorage.getItem(THEME_KEY) === 'dark' ? 'dark' : 'light';
  }

  private readBool(key: string, fallback: boolean): boolean {
    const stored = localStorage.getItem(key);
    return stored === null ? fallback : stored === 'true';
  }
}
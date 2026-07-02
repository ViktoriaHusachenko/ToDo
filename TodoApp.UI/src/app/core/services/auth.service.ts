import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

const TOKEN_KEY = 'auth_token';
const USER_KEY = 'auth_user';

export interface AuthUser {
    email: string;
    displayName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly apiUrl = `${environment.apiUrl}/auth`;

    private readonly _currentUser = signal<AuthUser | null>(this.readStoredUser());
    readonly currentUser = this._currentUser.asReadonly();
    readonly isAuthenticated = computed(() => this._currentUser() !== null);

    constructor(private http: HttpClient, private router: Router) { }

    login(request: LoginRequest): Observable<AuthResponse> {
        return this.http
            .post<AuthResponse>(`${this.apiUrl}/login`, request)
            .pipe(tap((res) => this.setSession(res)));
    }

    register(request: RegisterRequest): Observable<AuthResponse> {
        return this.http
            .post<AuthResponse>(`${this.apiUrl}/register`, request)
            .pipe(tap((res) => this.setSession(res)));
    }

    logout(): void {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USER_KEY);
        this._currentUser.set(null);
        this.router.navigate(['/login']);
    }

    getToken(): string | null {
        return localStorage.getItem(TOKEN_KEY);
    }

    private setSession(res: AuthResponse): void {
        localStorage.setItem(TOKEN_KEY, res.token);
        const user: AuthUser = { email: res.email, displayName: res.displayName };
        localStorage.setItem(USER_KEY, JSON.stringify(user));
        this._currentUser.set(user);
    }

    private readStoredUser(): AuthUser | null {
        const raw = localStorage.getItem(USER_KEY);
        return raw ? (JSON.parse(raw) as AuthUser) : null;
    }
}
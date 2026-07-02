import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    { path: 'login', loadComponent: () => import('./pages/login/login').then((m) => m.Login) },
    { path: 'register', loadComponent: () => import('./pages/register/register').then((m) => m.Register) },
    {
        path: '',
        loadComponent: () => import('./layout/shell/shell').then((m) => m.Shell),
        canActivate: [authGuard],
        children: [
            { path: '', redirectTo: 'tasks', pathMatch: 'full' },
            { path: 'tasks', loadComponent: () => import('./pages/tasks/tasks').then((m) => m.Tasks) },
            { path: 'important', loadComponent: () => import('./pages/important/important').then((m) => m.Important) },
            { path: 'planned', loadComponent: () => import('./pages/planned/planned').then((m) => m.Planned) },
            { path: 'completed', loadComponent: () => import('./pages/completed/completed').then((m) => m.Completed) },
            { path: 'settings', loadComponent: () => import('./pages/settings/settings').then((m) => m.Settings) },
            { path: 'profile', loadComponent: () => import('./pages/profile/profile').then((m) => m.Profile) }
        ]
    },
    { path: '**', redirectTo: 'tasks' }
];
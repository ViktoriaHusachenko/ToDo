import { Routes } from '@angular/router';

import { Tasks } from './pages/tasks/tasks';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Settings } from './pages/settings/settings';

export const routes: Routes = [

  {
    path: '',
    component: Tasks
  },

  {
    path: 'login',
    component: Login
  },

  {
    path: 'register',
    component: Register
  },

  {
    path: 'settings',
    component: Settings
  },

  {
    path: '**',
    redirectTo: ''
  }

];
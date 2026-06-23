import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'events', pathMatch: 'full' },

  // Public
  { path: 'login',       loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent) },
  { path: 'register',    loadComponent: () => import('./components/register/register.component').then(m => m.RegisterComponent) },
  { path: 'admin-login', loadComponent: () => import('./components/admin-login/admin-login.component').then(m => m.AdminLoginComponent) },
  { path: 'events',      loadComponent: () => import('./components/events/event-list/event-list.component').then(m => m.EventListComponent) },
  { path: 'events/:id',  loadComponent: () => import('./components/events/event-detail/event-detail.component').then(m => m.EventDetailComponent) },

  // Participant
  { path: 'my-events',           canActivate: [authGuard], loadComponent: () => import('./components/participant/my-events.component').then(m => m.MyEventsComponent) },
  { path: 'participant/dashboard', canActivate: [authGuard], loadComponent: () => import('./components/participant-dashboard/participant-dashboard.component').then(m => m.ParticipantDashboardComponent) },

  // Admin
  { path: 'admin/dashboard',           canActivate: [adminGuard], loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent) },
  { path: 'admin/events/new',           canActivate: [adminGuard], loadComponent: () => import('./components/events/event-form/event-form.component').then(m => m.EventFormComponent) },
  { path: 'admin/events/edit/:id',      canActivate: [adminGuard], loadComponent: () => import('./components/events/event-form/event-form.component').then(m => m.EventFormComponent) },
  { path: 'admin/sessions',             canActivate: [adminGuard], loadComponent: () => import('./components/sessions/session-admin-list/session-admin-list.component').then(m => m.SessionAdminListComponent) },
  { path: 'admin/sessions/new',         canActivate: [adminGuard], loadComponent: () => import('./components/sessions/session-form/session-form.component').then(m => m.SessionFormComponent) },
  { path: 'admin/sessions/edit/:id',    canActivate: [adminGuard], loadComponent: () => import('./components/sessions/session-form/session-form.component').then(m => m.SessionFormComponent) },
  { path: 'admin/speakers',             canActivate: [adminGuard], loadComponent: () => import('./components/speakers/speakers.component').then(m => m.SpeakersComponent) },
  { path: 'admin/speaker-assignments',  canActivate: [adminGuard], loadComponent: () => import('./components/speaker-assignments/speaker-assignments.component').then(m => m.SpeakerAssignmentsComponent) },
  { path: 'admin/categories',           canActivate: [adminGuard], loadComponent: () => import('./components/categories/categories.component').then(m => m.CategoriesComponent) },
  { path: 'admin/attendance',           canActivate: [adminGuard], loadComponent: () => import('./components/attendance/attendance.component').then(m => m.AttendanceComponent) },
  { path: 'admin/participants',         canActivate: [adminGuard], loadComponent: () => import('./components/participants/participants.component').then(m => m.ParticipantsComponent) },

  { path: '**', redirectTo: 'events' }
];

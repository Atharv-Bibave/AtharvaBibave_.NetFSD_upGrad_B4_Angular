# Virtual Event Management System – Angular Frontend (Project 4)

**Project:** B4_AtharvaBibave_upGradVirtualEventManagementSystem-IV  
**Submitted by:** Atharva Mahesh Bibave  
**Program:** upGrad .NET + Angular Full Stack Development (Enterprise PAT)

---

## Prerequisites

| Tool | Version |
|---|---|
| Node.js | 18.x or 20.x (LTS recommended) |
| npm | Comes with Node.js |
| Angular CLI | 21.x (`npm install -g @angular/cli`) |
| VS Code | Any recent version (recommended) |

> **Backend must be running first** before starting the Angular app.  
> See `README_Backend.md` in the Web API project.

---

## How to Run

```bash
# 1. Install dependencies
npm install

# 2. Start the dev server
ng serve

# 3. Open in browser
# → http://localhost:4200
```

> API calls are proxied to the backend via `proxy.conf.json`.  
> No CORS issues in development — the proxy handles it automatically.

---

## Login Credentials

| Role | Email | Password |
|---|---|---|
| Admin | admin@ems.com | Admin@123 |
| Participant | atharvabibave@gmail.com | Password@123 | 
or you can create new participant registration as well

---

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── login/              → Login page
│   │   ├── register/           → Participant registration
│   │   ├── events/
│   │   │   ├── event-list/     → Browse all events (public)
│   │   │   ├── event-detail/   → Single event view with sessions
│   │   │   └── event-form/     → Create / Edit event (Admin)
│   │   ├── sessions/
│   │   │   ├── session-list/   → All sessions (Admin)
│   │   │   └── session-form/   → Create / Edit session (Admin)
│   │   ├── speakers/           → Speaker management (Admin)
│   │   ├── categories/         → Category management (Admin)
│   │   ├── participants/       → Participant registrations (Admin)
│   │   ├── dashboard/          → Admin dashboard
│   │   └── navbar/             → Top navigation bar
│   │
│   ├── services/
│   │   ├── auth.service.ts     → Login, logout, JWT storage
│   │   ├── event.service.ts    → Event API calls
│   │   ├── session.service.ts  → Session API calls
│   │   ├── speaker.service.ts  → Speaker API calls
│   │   └── participant.service.ts → Registration API calls
│   │
│   ├── guards/
│   │   ├── auth.guard.ts       → Blocks unauthenticated users
│   │   ├── admin.guard.ts      → Blocks non-admin users
│   │   └── participant.guard.ts → Blocks non-participant users
│   │
│   ├── interceptors/
│   │   └── auth.interceptor.ts → Auto-attaches JWT to every request
│   │
│   ├── models/                 → TypeScript interfaces (Event, Session, etc.)
│   ├── app.routes.ts           → All route definitions
│   └── app.config.ts           → App bootstrap configuration
│
├── environments/
│   ├── environment.ts          → Dev: API base URL
│   └── environment.prod.ts     → Prod: API base URL
│
├── proxy.conf.json             → Dev proxy: forwards /api → backend
└── angular.json                → Angular CLI configuration
```

---

## Tech Stack

| Area | Technology |
|---|---|
| Framework | Angular 21 (Standalone Components) |
| Language | TypeScript |
| Styling | Bootstrap 5 |
| HTTP | Angular HttpClient + Functional Interceptor |
| Auth | JWT (stored in localStorage, decoded via `jwt-decode`) |
| Routing | Angular Router with lazy-loaded components |
| Route Protection | Functional Guards (`CanActivateFn`) |
| State | RxJS Observables + BehaviorSubject |

---

## Key Concepts

### Proxy (Development CORS)
`proxy.conf.json` forwards all `/api` calls from `localhost:4200` to the backend.  
This avoids browser CORS errors during development without changing the backend.

### JWT Role Decoding
ASP.NET Core uses long claim URIs (not simple keys like `"role"`).  
The app uses `jwt-decode` and reads:
- `http://schemas.microsoft.com/ws/2008/06/identity/claims/role` → role
- `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress` → email

### Standalone Components (Angular 21)
No `NgModule`. Each component declares its own imports.  
`app.config.ts` bootstraps the app with providers for `HttpClient`, `Router`, and the auth interceptor.

---

## Available Routes

| Route | Access | Description |
|---|---|---|
| `/` | Public | Home / Event listing |
| `/events` | Public | All events |
| `/events/:id` | Public | Event detail + sessions |
| `/login` | Public | Login page |
| `/register` | Public | Participant registration |
| `/admin/dashboard` | Admin | Admin overview |
| `/admin/events` | Admin | Manage events |
| `/admin/sessions` | Admin | Manage sessions |
| `/admin/speakers` | Admin | Manage speakers |
| `/admin/categories` | Admin | Manage categories |
| `/admin/participants` | Admin | View registrations |
| `/participant/my-events` | Participant | Registered events |

---

## Troubleshooting

| Issue | Fix |
|---|---|
| `npm install` errors | Use `npm install --legacy-peer-deps` |
| API calls returning 404 | Verify backend is running and `proxy.conf.json` target port matches |
| Login returns 401 | Check that backend is seeded with admin credentials |
| Blank page after login | Check browser console for JWT decode errors |
| `ng` command not found | Run `npm install -g @angular/cli` |

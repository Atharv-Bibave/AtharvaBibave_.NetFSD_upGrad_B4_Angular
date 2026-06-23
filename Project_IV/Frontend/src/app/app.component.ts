import { Component } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.component.html'
})
export class AppComponent {
  constructor(public auth: AuthService, private router: Router) {}
  get isLoggedIn(): boolean    { return this.auth.isLoggedIn(); }
  get isAdmin(): boolean       { return this.auth.getRole() === 'Admin'; }
  get isParticipant(): boolean { return this.auth.getRole() === 'Participant'; }
  get userName(): string       { return this.auth.getEmail() ?? ''; }
  logout(): void { this.auth.logout(); this.router.navigate(['/events']); }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ParticipantService } from '../../services/participant.service';
import { AuthService } from '../../services/auth.service';
import { ParticipantRegistration } from '../../models/participant.model';

@Component({
  selector: 'app-participant-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './participant-dashboard.component.html'
})
export class ParticipantDashboardComponent implements OnInit {
  registrations: ParticipantRegistration[] = [];
  isLoading = true;

  constructor(
    private participantService: ParticipantService,
    public auth: AuthService
  ) {}

  ngOnInit(): void {
    this.participantService.getMyEvents().subscribe({
      next: (regs) => { this.registrations = regs; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }

  get userName(): string    { return this.auth.getEmail() ?? ''; }
  get totalEvents(): number { return this.registrations.length; }
  get attended(): number    { return this.registrations.filter(r => r.isAttended).length; }
  get upcoming(): number    {
    return this.registrations.filter(r => !r.isAttended).length;
  }
  get recent(): ParticipantRegistration[] { return this.registrations.slice(0, 3); }
}

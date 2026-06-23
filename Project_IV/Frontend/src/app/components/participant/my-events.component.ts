import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ParticipantService } from '../../services/participant.service';
import { AuthService } from '../../services/auth.service';
import { ParticipantRegistration } from '../../models/participant.model';

@Component({
  selector: 'app-my-events',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-events.component.html'
})
export class MyEventsComponent implements OnInit {
  registrations: ParticipantRegistration[] = [];
  isLoading = false;
  errorMsg = '';
  successMsg = '';
  markingAttendance: { [id: string]: boolean } = {};

  constructor(private participantService: ParticipantService, public auth: AuthService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.participantService.getMyEvents().subscribe({
      next: (regs) => { this.registrations = regs; this.isLoading = false; },
      error: (err) => { this.errorMsg = err.error?.message ?? 'Failed to load.'; this.isLoading = false; }
    });
  }

  markAttendance(id: string, attended: boolean): void {
    this.markingAttendance[id] = true;
    this.participantService.markAttendance(id, attended).subscribe({
      next: (res) => {
        this.successMsg = res.message ?? 'Attendance updated!';
        this.markingAttendance[id] = false;
        this.load();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed.';
        this.markingAttendance[id] = false;
        setTimeout(() => this.errorMsg = '', 3000);
      }
    });
  }

  get email(): string { return this.auth.getEmail() ?? ''; }
}

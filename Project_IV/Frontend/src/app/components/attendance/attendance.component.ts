import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ParticipantService } from '../../services/participant.service';
import { EventService } from '../../services/event.service';
import { EventResponse } from '../../models/event.model';
import { ParticipantRegistration } from '../../models/participant.model';
import { ApiResponse } from '../../models/api-response.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './attendance.component.html'
})
export class AttendanceComponent implements OnInit {
  events: EventResponse[] = [];
  selectedEventId = '';
  loadingEvents = false;
  registrations: ParticipantRegistration[] = [];
  loadingRegs = false;
  markingId = '';
  errorMsg = '';
  successMsg = '';

  constructor(
    private eventService: EventService,
    private participantService: ParticipantService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadingEvents = true;
    this.eventService.getEvents(1, 100).subscribe({
      next: (r) => { this.events = r.items; this.loadingEvents = false; },
      error: () => { this.loadingEvents = false; }
    });
  }

  loadRegistrations(): void {
    if (!this.selectedEventId) return;
    this.loadingRegs = true;
    this.errorMsg = '';
    this.registrations = [];
    this.http.get<ApiResponse<ParticipantRegistration[]>>(
      `${environment.apiUrl}/participants/by-event/${this.selectedEventId}`
    ).pipe(map(r => r.data ?? [])).subscribe({
      next: (regs) => { this.registrations = regs; this.loadingRegs = false; },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed to load registrations.';
        this.loadingRegs = false;
      }
    });
  }

  markAttendance(registrationId: string, attended: boolean, email: string): void {
    this.errorMsg = '';
    this.successMsg = '';
    this.markingId = registrationId;
    this.participantService.markAttendance(registrationId, attended).subscribe({
      next: () => {
        this.successMsg = `Attendance updated for ${email}.`;
        this.markingId = '';
        const reg = this.registrations.find(r => r.id === registrationId);
        if (reg) reg.isAttended = attended;
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed to update attendance.';
        this.markingId = '';
      }
    });
  }

  get attendedCount(): number { return this.registrations.filter(r => r.isAttended).length; }
  get pendingCount(): number  { return this.registrations.filter(r => !r.isAttended).length; }

  get selectedEventName(): string {
    return this.events.find(e => e.eventId === this.selectedEventId)?.eventName ?? '';
  }
}

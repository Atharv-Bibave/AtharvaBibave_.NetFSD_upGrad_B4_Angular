import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { ParticipantService } from '../../services/participant.service';
import { UserResponse } from '../../models/auth.model';
import { ParticipantRegistration } from '../../models/participant.model';
import { ApiResponse } from '../../models/api-response.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-participants',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './participants.component.html'
})
export class ParticipantsComponent implements OnInit {
  participants: UserResponse[] = [];
  selectedEmail = '';
  allRegistrations: ParticipantRegistration[] = [];
  displayedRegistrations: ParticipantRegistration[] = [];
  loadingUsers = false;
  loadingRegs = false;
  markingId = '';
  errorMsg = '';
  successMsg = '';

  constructor(
    private auth: AuthService,
    private participantService: ParticipantService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadingUsers = true;
    this.auth.getAllUsers().subscribe({
      next: (res) => {
        const allUsers = res.data ?? [];
        this.participants = allUsers.filter(u => u.role === 'Participant');
        this.loadingUsers = false;
        // Auto-load all registrations for all participants
        this.loadAllRegistrations();
      },
      error: () => { this.loadingUsers = false; }
    });
  }

  loadAllRegistrations(): void {
    if (this.participants.length === 0) return;
    this.loadingRegs = true;

    const calls = this.participants.map(p =>
      this.http.get<ApiResponse<ParticipantRegistration[]>>(
        `${environment.apiUrl}/participants/by-participant/${encodeURIComponent(p.emailId)}`
      ).pipe(
        map(r => r.data ?? []),
        catchError(() => of([] as ParticipantRegistration[]))
      )
    );

    forkJoin(calls).subscribe({
      next: (results) => {
        this.allRegistrations = results.flat();
        this.displayedRegistrations = [...this.allRegistrations];
        this.loadingRegs = false;
      },
      error: () => { this.loadingRegs = false; }
    });
  }

  onFilterChange(): void {
    if (!this.selectedEmail) {
      this.displayedRegistrations = [...this.allRegistrations];
      return;
    }
    this.loadingRegs = true;
    this.http.get<ApiResponse<ParticipantRegistration[]>>(
      `${environment.apiUrl}/participants/by-participant/${encodeURIComponent(this.selectedEmail)}`
    ).pipe(map(r => r.data ?? [])).subscribe({
      next: (regs) => {
        this.displayedRegistrations = regs;
        this.loadingRegs = false;
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Could not load registrations.';
        this.loadingRegs = false;
      }
    });
  }

  clearFilter(): void {
    this.selectedEmail = '';
    this.displayedRegistrations = [...this.allRegistrations];
  }

  markAttendance(registrationId: string, attended: boolean): void {
    this.errorMsg = '';
    this.successMsg = '';
    this.markingId = registrationId;
    this.participantService.markAttendance(registrationId, attended).subscribe({
      next: () => {
        this.successMsg = 'Attendance updated.';
        this.markingId = '';
        const reg = this.displayedRegistrations.find(r => r.id === registrationId);
        if (reg) reg.isAttended = attended;
        const allReg = this.allRegistrations.find(r => r.id === registrationId);
        if (allReg) allReg.isAttended = attended;
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed.';
        this.markingId = '';
      }
    });
  }

  getUserName(email: string): string {
    return this.participants.find(p => p.emailId === email)?.userName ?? email;
  }
}

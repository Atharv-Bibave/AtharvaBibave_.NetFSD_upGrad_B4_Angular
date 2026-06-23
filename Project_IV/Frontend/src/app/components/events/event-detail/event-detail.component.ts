import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../../services/event.service';
import { SessionService } from '../../../services/session.service';
import { SpeakerService } from '../../../services/speaker.service';
import { ParticipantService } from '../../../services/participant.service';
import { AuthService } from '../../../services/auth.service';
import { EventResponse } from '../../../models/event.model';
import { SessionResponse } from '../../../models/session.model';
import { SpeakerResponse } from '../../../models/speaker.model';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './event-detail.component.html'
})
export class EventDetailComponent implements OnInit {
  event: EventResponse | null = null;
  sessions: SessionResponse[] = [];
  speakers: SpeakerResponse[] = [];
  totalPages = 0;
  currentPage = 1;
  isLoading = true;
  errorMsg = '';
  successMsg = '';
  registering = false;
  assignSessionId = '';
  selectedSpeakerId = '';
  assignLoading = false;
  assignError = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private sessionService: SessionService,
    private speakerService: SpeakerService,
    private participantService: ParticipantService,
    public auth: AuthService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadEvent(id);
    this.loadSessions(id);
    if (this.isAdmin) this.loadSpeakers();
  }

  loadEvent(id: string): void {
    this.eventService.getEventById(id).subscribe({
      next: (ev) => { this.event = ev; this.isLoading = false; },
      error: () => { this.errorMsg = 'Event not found.'; this.isLoading = false; }
    });
  }

  loadSessions(eventId: string, page = 1): void {
    this.sessionService.getByEvent(eventId, page).subscribe({
      next: (res) => { this.sessions = res.items; this.totalPages = res.totalPages; this.currentPage = page; }
    });
  }

  loadSpeakers(): void {
    this.speakerService.getAll().subscribe({ next: (list) => this.speakers = list });
  }

  deleteSession(id: string): void {
    if (!confirm('Delete this session?')) return;
    this.sessionService.delete(id).subscribe({
      next: () => { this.successMsg = 'Session deleted.'; this.loadSessions(this.event!.eventId, this.currentPage); },
      error: (err) => this.errorMsg = err.error?.message ?? 'Delete failed.'
    });
  }

  openAssignSpeaker(sessionId: string): void {
    this.assignSessionId = sessionId;
    this.selectedSpeakerId = '';
    this.assignError = '';
  }

  confirmAssignSpeaker(): void {
    if (!this.selectedSpeakerId) { this.assignError = 'Please select a speaker.'; return; }
    this.assignLoading = true;
    this.sessionService.assignSpeaker(this.assignSessionId, this.selectedSpeakerId).subscribe({
      next: () => {
        this.successMsg = 'Speaker assigned!';
        this.assignLoading = false;
        this.assignSessionId = '';
        this.loadSessions(this.event!.eventId, this.currentPage);
      },
      error: (err) => { this.assignError = err.error?.message ?? 'Failed.'; this.assignLoading = false; }
    });
  }

  registerForEvent(): void {
    if (!this.auth.isLoggedIn()) { this.router.navigate(['/login']); return; }
    this.registering = true;
    this.participantService.registerForEvent(this.event!.eventId).subscribe({
      next: (res) => { this.successMsg = res.message ?? 'Registered!'; this.registering = false; },
      error: (err) => { this.errorMsg = err.error?.message ?? 'Registration failed.'; this.registering = false; }
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.loadSessions(this.event!.eventId, page);
  }

  pages(): number[] { return Array.from({ length: this.totalPages }, (_, i) => i + 1); }
  get isAdmin(): boolean       { return this.auth.getRole() === 'Admin'; }
  get isParticipant(): boolean { return this.auth.getRole() === 'Participant'; }
}

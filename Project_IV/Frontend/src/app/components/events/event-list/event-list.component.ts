import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventService } from '../../../services/event.service';
import { ParticipantService } from '../../../services/participant.service';
import { AuthService } from '../../../services/auth.service';
import { EventResponse } from '../../../models/event.model';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-list.component.html'
})
export class EventListComponent implements OnInit {
  events: EventResponse[] = [];
  totalPages = 0;
  totalCount = 0;
  currentPage = 1;
  readonly pageSize = 6;
  isLoading = false;
  errorMsg = '';
  successMsg = '';
  registering: { [eventId: string]: boolean } = {};

  constructor(
    private eventService: EventService,
    private participantService: ParticipantService,
    public auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.eventService.getEvents(this.currentPage, this.pageSize).subscribe({
      next: (result) => {
        this.events = result.items;
        this.totalPages = result.totalPages;
        this.totalCount = result.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed to load events.';
        this.isLoading = false;
      }
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.load();
    window.scrollTo(0, 0);
  }

  pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  deleteEvent(id: string): void {
    if (!confirm('Delete this event? This cannot be undone.')) return;
    this.eventService.deleteEvent(id).subscribe({
      next: () => { this.successMsg = 'Event deleted.'; this.load(); },
      error: (err) => { this.errorMsg = err.error?.message ?? 'Delete failed.'; }
    });
  }

  registerForEvent(eventId: string): void {
    if (!this.auth.isLoggedIn()) { this.router.navigate(['/login']); return; }
    this.registering[eventId] = true;
    this.participantService.registerForEvent(eventId).subscribe({
      next: (res) => {
        this.successMsg = res.message ?? 'Registered successfully!';
        this.registering[eventId] = false;
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Registration failed.';
        this.registering[eventId] = false;
        setTimeout(() => this.errorMsg = '', 3000);
      }
    });
  }

  get isAdmin(): boolean       { return this.auth.getRole() === 'Admin'; }
  get isParticipant(): boolean { return this.auth.getRole() === 'Participant'; }
  get isLoggedIn(): boolean    { return this.auth.isLoggedIn(); }
}

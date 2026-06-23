import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SessionService } from '../../../services/session.service';
import { EventService } from '../../../services/event.service';
import { SessionResponse } from '../../../models/session.model';
import { EventResponse } from '../../../models/event.model';

@Component({
  selector: 'app-session-admin-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './session-admin-list.component.html'
})
export class SessionAdminListComponent implements OnInit {
  allSessions: SessionResponse[] = [];
  displayedSessions: SessionResponse[] = [];
  events: EventResponse[] = [];
  filterEventId = '';
  filterEventName = '';
  isLoading = false;
  errorMsg = '';
  successMsg = '';

  constructor(
    private sessionService: SessionService,
    private eventService: EventService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const eid = this.route.snapshot.queryParamMap.get('eventId');
    if (eid) this.filterEventId = eid;
    this.loadAll();
  }

  loadAll(): void {
    this.isLoading = true;
    this.errorMsg = '';
    this.eventService.getEvents(1, 100).subscribe({
      next: (res) => {
        this.events = res.items;
        if (res.items.length === 0) { this.isLoading = false; return; }

        // Load sessions for all events in parallel
        const calls = res.items.map(ev =>
          this.sessionService.getByEvent(ev.eventId, 1, 100).pipe(
            catchError(() => of({ items: [], totalPages: 0, totalCount: 0, page: 1, pageSize: 100 }))
          )
        );

        forkJoin(calls).subscribe({
          next: (results) => {
            this.allSessions = results.flatMap(r => r.items);
            this.applyFilter();
            this.isLoading = false;
          },
          error: () => { this.isLoading = false; }
        });
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed to load sessions.';
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    if (!this.filterEventId) {
      this.displayedSessions = [...this.allSessions];
      this.filterEventName = '';
    } else {
      this.displayedSessions = this.allSessions.filter(s => s.eventId === this.filterEventId);
      this.filterEventName = this.events.find(e => e.eventId === this.filterEventId)?.eventName ?? '';
    }
  }

  onFilterChange(): void { this.applyFilter(); }

  clearFilter(): void {
    this.filterEventId = '';
    this.applyFilter();
  }

  getEventName(eventId: string): string {
    return this.events.find(e => e.eventId === eventId)?.eventName ?? eventId;
  }

  delete(id: string): void {
    if (!confirm('Delete this session?')) return;
    this.sessionService.delete(id).subscribe({
      next: () => {
        this.successMsg = 'Session deleted.';
        this.allSessions = this.allSessions.filter(s => s.sessionId !== id);
        this.applyFilter();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => { this.errorMsg = err.error?.message ?? 'Delete failed.'; }
    });
  }
}

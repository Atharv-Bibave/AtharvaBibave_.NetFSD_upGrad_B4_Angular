import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EventService } from '../../services/event.service';
import { SpeakerService } from '../../services/speaker.service';
import { CategoryService } from '../../services/category.service';
import { SessionService } from '../../services/session.service';
import { AuthService } from '../../services/auth.service';
import { UserResponse } from '../../models/auth.model';
import { EventResponse } from '../../models/event.model';
import { SessionResponse } from '../../models/session.model';

export interface EventWithSessions {
  event: EventResponse;
  sessions: SessionResponse[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  isLoading = true;
  totalEvents = 0;
  activeEvents = 0;
  inactiveEvents = 0;
  totalSpeakers = 0;
  totalCategories = 0;
  totalUsers = 0;
  eventsWithSessions: EventWithSessions[] = [];
  errorMsg = '';

  constructor(
    private eventService: EventService,
    private speakerService: SpeakerService,
    private categoryService: CategoryService,
    private sessionService: SessionService,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    forkJoin({
      events:     this.eventService.getEvents(1, 100),
      speakers:   this.speakerService.getAll(),
      categories: this.categoryService.getAll(),
      users:      this.auth.getAllUsers()
    }).subscribe({
      next: (res) => {
        const allEvents      = res.events.items;
        this.totalEvents     = res.events.totalCount;
        this.activeEvents    = allEvents.filter(e => e.status === 'Active').length;
        this.inactiveEvents  = allEvents.filter(e => e.status !== 'Active').length;
        this.totalSpeakers   = res.speakers.length;
        this.totalCategories = res.categories.length;
        this.totalUsers      = (res.users.data ?? []).length;

        // Load sessions for each event
        const top5 = allEvents.slice(0, 5);
        if (top5.length === 0) { this.isLoading = false; return; }

        const sessionCalls = top5.map(ev =>
          this.sessionService.getByEvent(ev.eventId, 1, 100).pipe(
            catchError(() => of({ items: [], totalPages: 0, totalCount: 0, page: 1, pageSize: 100 }))
          )
        );

        forkJoin(sessionCalls).subscribe({
          next: (results) => {
            this.eventsWithSessions = top5.map((ev, i) => ({
              event: ev,
              sessions: (results[i].items ?? []).sort(
                (a, b) => new Date(a.sessionStart).getTime() - new Date(b.sessionStart).getTime()
              )
            }));
            this.isLoading = false;
          },
          error: () => {
            this.eventsWithSessions = top5.map(ev => ({ event: ev, sessions: [] }));
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        this.errorMsg  = err.error?.message ?? 'Failed to load dashboard.';
        this.isLoading = false;
      }
    });
  }

  get adminEmail(): string { return this.auth.getEmail() ?? 'Admin'; }
}

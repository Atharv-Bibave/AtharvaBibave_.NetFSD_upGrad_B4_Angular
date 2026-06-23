import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SpeakerService } from '../../services/speaker.service';
import { EventService } from '../../services/event.service';
import { SessionService } from '../../services/session.service';
import { SpeakerResponse } from '../../models/speaker.model';
import { SessionResponse } from '../../models/session.model';
import { EventResponse } from '../../models/event.model';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface SpeakerWithSessions {
  speaker: SpeakerResponse;
  sessions: SessionResponse[];
}

@Component({
  selector: 'app-speaker-assignments',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './speaker-assignments.component.html'
})
export class SpeakerAssignmentsComponent implements OnInit {
  speakerAssignments: SpeakerWithSessions[] = [];
  unassignedSessions: SessionResponse[] = [];
  isLoading = false;
  errorMsg = '';
  successMsg = '';

  constructor(
    private speakerService: SpeakerService,
    private eventService: EventService,
    private sessionService: SessionService
  ) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.errorMsg = '';

    forkJoin({
      speakers: this.speakerService.getAll(),
      events:   this.eventService.getEvents(1, 100)
    }).subscribe({
      next: ({ speakers, events }) => {
        // Load sessions for all events
        const sessionCalls = events.items.map(ev =>
          this.sessionService.getByEvent(ev.eventId, 1, 100).pipe(catchError(() => of({ items: [], totalPages: 0, totalCount: 0, page: 1, pageSize: 100 })))
        );

        if (sessionCalls.length === 0) {
          this.buildAssignments(speakers, []);
          this.isLoading = false;
          return;
        }

        forkJoin(sessionCalls).subscribe({
          next: (results) => {
            const allSessions = results.flatMap(r => r.items);
            this.buildAssignments(speakers, allSessions);
            this.isLoading = false;
          },
          error: () => { this.isLoading = false; }
        });
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed to load data.';
        this.isLoading = false;
      }
    });
  }

  buildAssignments(speakers: SpeakerResponse[], sessions: SessionResponse[]): void {
    this.speakerAssignments = speakers.map(sp => ({
      speaker: sp,
      sessions: sessions
        .filter(s => s.speakerId === sp.speakerId)
        .sort((a, b) => new Date(a.sessionStart).getTime() - new Date(b.sessionStart).getTime())
    }));
    this.unassignedSessions = sessions
      .filter(s => !s.speakerId)
      .sort((a, b) => new Date(a.sessionStart).getTime() - new Date(b.sessionStart).getTime());
  }
}

import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SessionService } from '../../../services/session.service';
import { SessionResponse } from '../../../models/session.model';

@Component({
  selector: 'app-session-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './session-list.component.html'
})
export class SessionListComponent implements OnInit, OnChanges {
  @Input() eventId = '';
  sessions: SessionResponse[] = [];
  isLoading = false;

  constructor(private sessionService: SessionService) {}

  ngOnInit(): void { if (this.eventId) this.load(); }
  ngOnChanges(): void { if (this.eventId) this.load(); }

  load(): void {
    this.isLoading = true;
    this.sessionService.getByEvent(this.eventId).subscribe({
      next: (res) => { this.sessions = res.items; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }
}

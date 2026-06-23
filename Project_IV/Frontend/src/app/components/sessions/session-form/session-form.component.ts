import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SessionService } from '../../../services/session.service';
import { SpeakerService } from '../../../services/speaker.service';
import { EventService } from '../../../services/event.service';
import { SpeakerResponse } from '../../../models/speaker.model';
import { EventResponse } from '../../../models/event.model';

function dateRangeValidator(group: AbstractControl) {
  const start = group.get('sessionStart')?.value;
  const end   = group.get('sessionEnd')?.value;
  if (start && end && new Date(end) <= new Date(start)) return { dateRange: true };
  return null;
}

@Component({
  selector: 'app-session-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './session-form.component.html'
})
export class SessionFormComponent implements OnInit {
  form: FormGroup;
  speakers: SpeakerResponse[] = [];
  events: EventResponse[] = [];
  isEditMode = false;
  sessionId: string | null = null;
  originalEventId = '';
  loading = false;
  submitting = false;
  errorMsg = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private sessionService: SessionService,
    private speakerService: SpeakerService,
    private eventService: EventService
  ) {
    this.form = this.fb.group({
      eventId:      ['', Validators.required],
      sessionTitle: ['', [Validators.required, Validators.maxLength(50)]],
      speakerId:    [''],
      description:  ['', Validators.maxLength(500)],
      sessionStart: ['', Validators.required],
      sessionEnd:   ['', Validators.required],
      sessionUrl:   ['']
    }, { validators: dateRangeValidator });
  }

  ngOnInit(): void {
    this.speakerService.getAll().subscribe({ next: (s) => this.speakers = s });
    this.eventService.getEvents(1, 100).subscribe({ next: (r) => this.events = r.items });

    const preEventId = this.route.snapshot.queryParamMap.get('eventId');
    if (preEventId) this.form.patchValue({ eventId: preEventId });

    this.sessionId = this.route.snapshot.paramMap.get('id');
    if (this.sessionId) {
      this.isEditMode = true;
      this.loading = true;
      this.sessionService.getById(this.sessionId).subscribe({
        next: (s) => {
          this.originalEventId = s.eventId;
          this.form.patchValue({
            eventId:      s.eventId,
            sessionTitle: s.sessionTitle,
            speakerId:    s.speakerId ?? '',
            description:  s.description ?? '',
            sessionStart: this.isoToDatetimeLocal(s.sessionStart),
            sessionEnd:   this.isoToDatetimeLocal(s.sessionEnd),
            sessionUrl:   s.sessionUrl ?? ''
          });
          this.loading = false;
        },
        error: () => {
          this.errorMsg = 'Could not load session. Please go back and try again.';
          this.loading = false;
        }
      });
    }
  }

  // Convert ISO string from backend to datetime-local input value
  // Keeps the exact date/time as stored — no timezone conversion
  isoToDatetimeLocal(iso: string): string {
    // Backend sends e.g. "2026-05-08T13:00:00" — take first 16 chars
    return iso.substring(0, 16);
  }

  getSelectedEventDate(): string {
    const eventId = this.isEditMode ? this.originalEventId : this.form.value.eventId;
    const ev = this.events.find(e => e.eventId === eventId);
    return ev ? ev.eventDate.substring(0, 10) : '';
  }

  f(name: string) { return this.form.get(name); }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.submitting = true;
    this.errorMsg = '';

    const val = { ...this.form.value };
    if (this.isEditMode) val.eventId = this.originalEventId;

    const navigateEventId = val.eventId;

    const call = this.isEditMode
      ? this.sessionService.update(this.sessionId!, val)
      : this.sessionService.add(val);

    call.subscribe({
      next: () => {
        this.router.navigate(['/events', navigateEventId]);
      },
      error: (err) => {
        const msg: string = err.error?.message ?? '';
        if (msg.toLowerCase().includes('event date') || msg.toLowerCase().includes('sessionstart')) {
          this.errorMsg = `Session date must be on or after the event date (${this.getSelectedEventDate()}).`;
        } else if (msg.toLowerCase().includes('url')) {
          this.errorMsg = 'Invalid session URL. Use full URL like https://zoom.us/... or leave blank.';
        } else if (msg.toLowerCase().includes('end') && msg.toLowerCase().includes('start')) {
          this.errorMsg = 'Session End must be after Session Start.';
        } else if (msg.toLowerCase().includes('not found')) {
          this.errorMsg = 'Session not found. It may have been deleted. Go back and refresh.';
        } else {
          this.errorMsg = msg || 'Failed to save. Check all fields are valid.';
        }
        this.submitting = false;
      }
    });
  }
}

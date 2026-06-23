import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventService } from '../../../services/event.service';
import { CategoryService } from '../../../services/category.service';
import { CategoryResponse } from '../../../models/category.model';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './event-form.component.html'
})
export class EventFormComponent implements OnInit {
  form: FormGroup;
  categories: CategoryResponse[] = [];
  isEditMode = false;
  eventId: string | null = null;
  loading = false;
  submitting = false;
  errorMsg = '';
  today = new Date().toISOString().split('T')[0];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private categoryService: CategoryService
  ) {
    this.form = this.fb.group({
      eventName:   ['', [Validators.required, Validators.minLength(1), Validators.maxLength(50)]],
      categoryId:  ['', Validators.required],
      eventDate:   ['', Validators.required],
      description: ['', Validators.maxLength(500)],
      status:      ['Active', Validators.required]
    });
  }

  ngOnInit(): void {
    this.categoryService.getAll().subscribe({ next: (cats) => this.categories = cats });
    this.eventId = this.route.snapshot.paramMap.get('id');
    if (this.eventId) {
      this.isEditMode = true;
      this.loading = true;
      this.eventService.getEventById(this.eventId).subscribe({
        next: (ev) => {
          this.form.patchValue({
            eventName: ev.eventName, categoryId: ev.categoryId,
            eventDate: ev.eventDate.substring(0, 10),
            description: ev.description ?? '', status: ev.status
          });
          this.loading = false;
        },
        error: () => { this.errorMsg = 'Event not found.'; this.loading = false; }
      });
    }
  }

  f(name: string) { return this.form.get(name); }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.submitting = true;
    this.errorMsg = '';
    const call = this.isEditMode
      ? this.eventService.updateEvent(this.eventId!, this.form.value)
      : this.eventService.createEvent(this.form.value);
    call.subscribe({
      next: () => this.router.navigate(['/events']),
      error: (err) => { this.errorMsg = err.error?.message ?? 'Failed to save event.'; this.submitting = false; }
    });
  }
}

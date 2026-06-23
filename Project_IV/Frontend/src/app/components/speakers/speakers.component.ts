import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SpeakerService } from '../../services/speaker.service';
import { SpeakerResponse } from '../../models/speaker.model';

@Component({
  selector: 'app-speakers',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './speakers.component.html'
})
export class SpeakersComponent implements OnInit {
  speakers: SpeakerResponse[] = [];
  form: FormGroup;
  isLoading = false;
  submitting = false;
  deletingId = '';
  showForm = false;
  errorMsg = '';
  successMsg = '';

  constructor(private speakerService: SpeakerService, private fb: FormBuilder) {
    this.form = this.fb.group({
      speakerName: ['', [Validators.required, Validators.maxLength(50)]]
    });
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.speakerService.getAll().subscribe({
      next: (s) => { this.speakers = s; this.isLoading = false; },
      error: () => this.isLoading = false
    });
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.errorMsg = '';
    this.successMsg = '';
    this.submitting = true;
    this.speakerService.add(this.form.value).subscribe({
      next: (res) => {
        this.successMsg = res.message ?? 'Speaker added!';
        this.form.reset();
        this.submitting = false;
        this.showForm = false;
        this.load();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed to add speaker.';
        this.submitting = false;
      }
    });
  }

  delete(id: string, name: string): void {
    if (!confirm(`Delete speaker "${name}"?`)) return;
    this.errorMsg = '';
    this.successMsg = '';
    this.deletingId = id;
    this.speakerService.delete(id).subscribe({
      next: () => {
        this.successMsg = `"${name}" deleted.`;
        this.deletingId = '';
        this.load();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Delete failed.';
        this.deletingId = '';
      }
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../services/category.service';
import { CategoryResponse } from '../../models/category.model';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories.component.html'
})
export class CategoriesComponent implements OnInit {
  categories: CategoryResponse[] = [];
  form: FormGroup;
  isLoading = false;
  submitting = false;
  deletingId = '';
  errorMsg = '';
  successMsg = '';

  constructor(private categoryService: CategoryService, private fb: FormBuilder) {
    this.form = this.fb.group({
      categoryName: ['', [Validators.required, Validators.maxLength(50)]]
    });
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.errorMsg = '';
    this.categoryService.getAll().subscribe({
      next: (c) => { this.categories = c; this.isLoading = false; },
      error: () => this.isLoading = false
    });
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.errorMsg = '';
    this.successMsg = '';
    this.submitting = true;
    this.categoryService.add(this.form.value).subscribe({
      next: (res) => {
        this.successMsg = res.message ?? 'Category added!';
        this.form.reset();
        this.submitting = false;
        this.load();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Failed to add category.';
        this.submitting = false;
      }
    });
  }

  delete(id: string, name: string): void {
    if (!confirm(`Delete category "${name}"?\nEvents using this category may be affected.`)) return;
    this.errorMsg = '';
    this.successMsg = '';
    this.deletingId = id;
    this.categoryService.delete(id).subscribe({
      next: () => {
        this.successMsg = `"${name}" deleted successfully.`;
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

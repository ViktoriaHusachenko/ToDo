import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CategoryService } from '../../core/services/category.service';
import { TaskService } from '../../core/services/task.service';
import { CategoryDto } from '../../core/models/category.model';

const COLOR_PALETTE = ['#0078d4', '#d13438', '#ca5010', '#107c10', '#5c2d91', '#e3008c', '#008272', '#605e5c'];

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, ReactiveFormsModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar {
  private readonly categoryService = inject(CategoryService);
  private readonly taskService = inject(TaskService);
  private readonly fb = inject(FormBuilder);

  readonly categories = signal<CategoryDto[]>([]);
  readonly taskCounts = signal<Record<string, number>>({});
  readonly palette = COLOR_PALETTE;

  readonly isFormOpen = signal(false);
  readonly editingCategoryId = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    color: [COLOR_PALETTE[0]]
  });

  constructor() {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe((categories) => {
      this.categories.set(categories);
      this.loadTaskCounts();
    });
  }

  private loadTaskCounts(): void {
    this.taskService.getAll().subscribe((tasks) => {
      const counts: Record<string, number> = {};
      for (const task of tasks) {
        if (!task.categoryName) continue;
        const category = this.categories().find((c) => c.name === task.categoryName);
        if (category) counts[category.id] = (counts[category.id] ?? 0) + 1;
      }
      this.taskCounts.set(counts);
    });
  }

  openCreateForm(): void {
    this.editingCategoryId.set(null);
    this.form.reset({ name: '', color: this.palette[0] });
    this.isFormOpen.set(true);
  }

  openEditForm(category: CategoryDto, event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.editingCategoryId.set(category.id);
    this.form.reset({ name: category.name, color: category.color || this.palette[0] });
    this.isFormOpen.set(true);
  }

  closeForm(): void {
    this.isFormOpen.set(false);
  }

  submit(): void {
    if (this.form.invalid) return;

    const dto = this.form.getRawValue();
    const id = this.editingCategoryId();

    if (id) {
      this.categoryService.update(id, dto).subscribe(() => {
        this.closeForm();
        this.loadCategories();
      });
    } else {
      this.categoryService.create(dto).subscribe(() => {
        this.closeForm();
        this.loadCategories();
      });
    }
  }

  deleteCategory(category: CategoryDto, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (!confirm(`Видалити категорію "${category.name}"?`)) return;

    this.categoryService.delete(category.id).subscribe(() => this.loadCategories());
  }
}
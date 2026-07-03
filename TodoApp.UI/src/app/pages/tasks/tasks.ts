import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { TaskService } from '../../core/services/task.service';
import { CategoryService } from '../../core/services/category.service';
import { TaskImportanceService } from '../../core/services/task-importance.service';
import { TaskItem } from '../../shared/components/task-item/task-item';
import { TaskDto } from '../../core/models/task.model';
import { TaskPriority } from '../../core/models/task-priority.enum';
import { CategoryDto } from '../../core/models/category.model';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [TaskItem, ReactiveFormsModule],
  templateUrl: './tasks.html',
  styleUrl: './tasks.scss'
})
export class Tasks {
  private readonly taskService = inject(TaskService);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  readonly importanceService = inject(TaskImportanceService);

  private readonly queryParamMap = toSignal(this.route.queryParamMap, { initialValue: null });

  private readonly allTasks = signal<TaskDto[]>([]);
  readonly categories = signal<CategoryDto[]>([]);
  readonly isLoading = signal(true);
  readonly searchTerm = signal('');

  readonly TaskPriority = TaskPriority;
  readonly isModalOpen = signal(false);
  readonly editingTaskId = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    title: ['', Validators.required],
    description: [''],
    dueDate: [''],
    priority: [TaskPriority.Medium],
    categoryId: ['']
  });

  readonly viewTitle = computed(() => {
    const view = this.queryParamMap()?.get('view');
    const categoryId = this.queryParamMap()?.get('categoryId');
    if (view === 'today') return 'Сьогодні';
    if (categoryId) return this.categories().find((c) => c.id === categoryId)?.name ?? 'Категорія';
    return 'Усі задачі';
  });

  readonly filteredTasks = computed(() => {
    const view = this.queryParamMap()?.get('view');
    const categoryId = this.queryParamMap()?.get('categoryId');
    const term = this.searchTerm().trim().toLowerCase();
    const categoryName = categoryId ? this.categories().find((c) => c.id === categoryId)?.name : null;

    return this.allTasks()
      .filter((t) => !t.isCompleted)
      .filter((t) => (categoryName ? t.categoryName === categoryName : true))
      .filter((t) => (view === 'today' ? this.isToday(t) : true))
      .filter((t) => (term ? t.title.toLowerCase().includes(term) : true));
  });

  constructor() {
    this.categoryService.getAll().subscribe((categories) => this.categories.set(categories));
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading.set(true);
    this.taskService.getAll().subscribe({
      next: (tasks) => { this.allTasks.set(tasks); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onSearchChange(value: string): void {
    this.searchTerm.set(value);
  }

  openCreateModal(): void {
    this.editingTaskId.set(null);
    this.form.reset({ title: '', description: '', dueDate: '', priority: TaskPriority.Medium, categoryId: '' });
    this.isModalOpen.set(true);
  }

  openEditModal(task: TaskDto): void {
    this.editingTaskId.set(task.id);
    this.form.reset({
      title: task.title,
      description: task.description ?? '',
      dueDate: task.dueDate ? task.dueDate.substring(0, 10) : '',
      priority: task.priority,
      categoryId: this.categoryIdByName(task.categoryName)
    });
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
  }

  submit(): void {
    if (this.form.invalid) return;

    const raw = this.form.getRawValue();
    const dueDate = raw.dueDate ? new Date(raw.dueDate).toISOString() : null;
    const categoryId = raw.categoryId || null;
    const id = this.editingTaskId();

    if (id) {
      const dto = {
        id,
        title: raw.title,
        description: raw.description || null,
        dueDate,
        isCompleted: this.allTasks().find((t) => t.id === id)?.isCompleted ?? false,
        priority: raw.priority,
        categoryId
      };
      this.taskService.update(id, dto).subscribe(() => { this.closeModal(); this.loadTasks(); });
    } else {
      const dto = { title: raw.title, description: raw.description || null, dueDate, priority: raw.priority, categoryId };
      this.taskService.create(dto).subscribe(() => { this.closeModal(); this.loadTasks(); });
    }
  }

  onToggleComplete(task: TaskDto): void {
    const action$ = task.isCompleted ? this.taskService.uncomplete(task.id) : this.taskService.complete(task.id);
    action$.subscribe(() => this.loadTasks());
  }

  onToggleImportant(taskId: string): void {
    this.importanceService.toggle(taskId);
  }

  onDelete(taskId: string): void {
    if (!confirm('Видалити задачу?')) return;
    this.taskService.delete(taskId).subscribe(() => this.loadTasks());
  }

  private isToday(task: TaskDto): boolean {
    if (!task.dueDate) return false;
    return new Date(task.dueDate).toDateString() === new Date().toDateString();
  }

  private categoryIdByName(name?: string | null): string {
    if (!name) return '';
    return this.categories().find((c) => c.name === name)?.id ?? '';
  }
}
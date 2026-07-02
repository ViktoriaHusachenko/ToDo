import { Component, computed, inject, signal } from '@angular/core';
import { TaskService } from '../../core/services/task.service';
import { TaskImportanceService } from '../../core/services/task-importance.service';
import { TaskDto } from '../../core/models/task.model';
import { TaskItem } from '../../shared/task-item/task-item';

@Component({
    selector: 'app-planned',
    standalone: true,
    imports: [TaskItem],
    templateUrl: './planned.html',
    styleUrl: './planned.scss'
})
export class Planned {
    private readonly taskService = inject(TaskService);
    readonly importanceService = inject(TaskImportanceService);

    private readonly tasks = signal<TaskDto[]>([]);
    readonly isLoading = signal(true);

    readonly plannedTasks = computed(() =>
        this.tasks()
            .filter((t) => !!t.dueDate)
            .sort((a, b) => new Date(a.dueDate!).getTime() - new Date(b.dueDate!).getTime())
    );

    readonly overdueTasks = computed(() => this.plannedTasks().filter((t) => this.isOverdue(t)));
    readonly todayTasks = computed(() => this.plannedTasks().filter((t) => this.isToday(t) && !this.isOverdue(t)));
    readonly upcomingTasks = computed(() =>
        this.plannedTasks().filter((t) => !this.isOverdue(t) && !this.isToday(t))
    );

    constructor() {
        this.load();
    }

    load(): void {
        this.isLoading.set(true);
        this.taskService.getAll().subscribe({
            next: (tasks) => { this.tasks.set(tasks); this.isLoading.set(false); },
            error: () => this.isLoading.set(false)
        });
    }

    isImportant(taskId: string): boolean {
        return this.importanceService.isImportant(taskId);
    }

    onToggleComplete(task: TaskDto): void {
        const action$ = task.isCompleted ? this.taskService.uncomplete(task.id) : this.taskService.complete(task.id);
        action$.subscribe(() => this.load());
    }

    onToggleImportant(taskId: string): void {
        this.importanceService.toggle(taskId);
    }

    onDelete(taskId: string): void {
        this.taskService.delete(taskId).subscribe(() => this.load());
    }

    private isOverdue(task: TaskDto): boolean {
        if (!task.dueDate || task.isCompleted) return false;
        return new Date(task.dueDate) < this.startOfToday();
    }

    private isToday(task: TaskDto): boolean {
        if (!task.dueDate) return false;
        return new Date(task.dueDate).toDateString() === this.startOfToday().toDateString();
    }

    private startOfToday(): Date {
        const now = new Date();
        return new Date(now.getFullYear(), now.getMonth(), now.getDate());
    }
}
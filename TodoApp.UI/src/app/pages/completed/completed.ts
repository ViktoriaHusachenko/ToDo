import { Component, computed, inject, signal } from '@angular/core';
import { TaskService } from '../../core/services/task.service';
import { TaskImportanceService } from '../../core/services/task-importance.service';
import { TaskItem } from '../../shared/components/task-item/task-item';
import { TaskDto } from '../../core/models/task.model';

@Component({
    selector: 'app-completed',
    standalone: true,
    imports: [TaskItem],
    templateUrl: './completed.html',
    styleUrl: './completed.scss'
})
export class Completed {
    private readonly taskService = inject(TaskService);
    readonly importanceService = inject(TaskImportanceService);

    private readonly tasks = signal<TaskDto[]>([]);
    readonly isLoading = signal(true);

    readonly completedTasks = computed(() => this.tasks().filter((t) => t.isCompleted));

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
        this.taskService.uncomplete(task.id).subscribe(() => this.load());
    }

    onToggleImportant(taskId: string): void {
        this.importanceService.toggle(taskId);
    }

    onDelete(taskId: string): void {
        this.taskService.delete(taskId).subscribe(() => this.load());
    }
}
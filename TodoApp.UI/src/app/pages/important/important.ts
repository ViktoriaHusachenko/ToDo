import { Component, computed, inject, signal } from '@angular/core';
import { TaskService } from '../../core/services/task.service';
import { TaskImportanceService } from '../../core/services/task-importance.service';
import { TaskDto } from '../../core/models/task.model';
import { TaskItem } from '../../shared/components/task-item/task-item';

@Component({
    selector: 'app-important',
    standalone: true,
    imports: [TaskItem],
    templateUrl: './important.html',
    styleUrl: './important.scss'
})
export class Important {
    private readonly taskService = inject(TaskService);
    readonly importanceService = inject(TaskImportanceService);

    private readonly tasks = signal<TaskDto[]>([]);
    readonly isLoading = signal(true);

    readonly importantTasks = computed(() =>
        this.tasks().filter((t) => this.importanceService.importantIds().has(t.id))
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
}
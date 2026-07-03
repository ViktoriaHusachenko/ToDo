import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TaskDto } from '../../../core/models/task.model';
import { TaskPriority } from '../../../core/models/task-priority.enum';

@Component({
    selector: 'app-task-item',
    standalone: true,
    imports: [DatePipe],
    templateUrl: './task-item.html',
    styleUrl: './task-item.scss'
})
export class TaskItem {
    readonly task = input.required<TaskDto>();
    readonly isImportant = input(false);

    readonly toggleComplete = output<void>();
    readonly toggleImportant = output<void>();
    readonly deleteTask = output<void>();
    readonly editTask = output<void>(); 
    readonly TaskPriority = TaskPriority;

    get isOverdue(): boolean {
        const t = this.task();
        if (!t.dueDate || t.isCompleted) return false;
        const startOfToday = new Date();
        startOfToday.setHours(0, 0, 0, 0);
        return new Date(t.dueDate) < startOfToday;
    }
}
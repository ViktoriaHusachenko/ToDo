import { TaskPriority } from './task-priority.enum';

export interface TaskDto {
    id: string;
    title: string;
    description?: string | null;
    dueDate?: string | null;
    isCompleted: boolean;
    priority: TaskPriority;
    categoryName?: string | null;
}

export interface CreateTaskDto {
    title: string;
    description?: string | null;
    dueDate?: string | null;
    priority: TaskPriority;
    categoryId?: string | null;
}

export interface UpdateTaskDto {
    id: string;
    title: string;
    description?: string | null;
    dueDate?: string | null;
    isCompleted: boolean;
    priority: TaskPriority;
    categoryId?: string | null;
}
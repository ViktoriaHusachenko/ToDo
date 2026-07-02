import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'important_task_ids';

@Injectable({ providedIn: 'root' })
export class TaskImportanceService {
    private readonly _importantIds = signal<Set<string>>(this.readStored());
    readonly importantIds = this._importantIds.asReadonly();

    isImportant(taskId: string): boolean {
        return this._importantIds().has(taskId);
    }

    toggle(taskId: string): void {
        const current = new Set(this._importantIds());
        current.has(taskId) ? current.delete(taskId) : current.add(taskId);
        this._importantIds.set(current);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(Array.from(current)));
    }

    private readStored(): Set<string> {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            return raw ? new Set(JSON.parse(raw) as string[]) : new Set();
        } catch {
            return new Set();
        }
    }
}
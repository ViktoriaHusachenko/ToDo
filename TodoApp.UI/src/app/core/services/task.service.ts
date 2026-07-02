import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { forkJoin, map, Observable, of, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/paged-result.model';
import { TaskDto, CreateTaskDto, UpdateTaskDto } from '../models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
    private readonly apiUrl = `${environment.apiUrl}/tasks`;

    constructor(private http: HttpClient) { }

    getPaged(
        pageNumber = 1,
        pageSize = 10,
        search?: string,
        categoryId?: string
    ): Observable<PagedResult<TaskDto>> {
        let params = new HttpParams()
            .set('pageNumber', pageNumber)
            .set('pageSize', pageSize);

        if (search) params = params.set('search', search);
        if (categoryId) params = params.set('categoryId', categoryId);

        return this.http.get<PagedResult<TaskDto>>(this.apiUrl, { params });
    }

    getAll(search?: string, categoryId?: string): Observable<TaskDto[]> {
        const pageSize = 50;

        return this.getPaged(1, pageSize, search, categoryId).pipe(
            switchMap((first) => {
                if (first.totalPages <= 1) {
                    return of(first.items);
                }

                const remainingPages = Array.from({ length: first.totalPages - 1 }, (_, i) => i + 2);
                const requests = remainingPages.map((page) => this.getPaged(page, pageSize, search, categoryId));

                return forkJoin(requests).pipe(
                    map((results) => [...first.items, ...results.flatMap((r) => r.items)])
                );
            })
        );
    }

    getById(id: string): Observable<TaskDto> {
        return this.http.get<TaskDto>(`${this.apiUrl}/${id}`);
    }

    create(dto: CreateTaskDto): Observable<TaskDto> {
        return this.http.post<TaskDto>(this.apiUrl, dto);
    }

    update(id: string, dto: UpdateTaskDto): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    complete(id: string): Observable<void> {
        return this.http.patch<void>(`${this.apiUrl}/${id}/complete`, {});
    }

    uncomplete(id: string): Observable<void> {
        return this.http.patch<void>(`${this.apiUrl}/${id}/uncomplete`, {});
    }
}
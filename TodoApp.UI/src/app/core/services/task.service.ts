import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
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
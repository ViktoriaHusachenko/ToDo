import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CategoryDto, CreateCategoryDto } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
    private readonly apiUrl = `${environment.apiUrl}/categories`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<CategoryDto[]> {
        return this.http.get<CategoryDto[]>(this.apiUrl);
    }

    getById(id: string): Observable<CategoryDto> {
        return this.http.get<CategoryDto>(`${this.apiUrl}/${id}`);
    }

    create(dto: CreateCategoryDto): Observable<CategoryDto> {
        return this.http.post<CategoryDto>(this.apiUrl, dto);
    }

    update(id: string, dto: CreateCategoryDto): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}
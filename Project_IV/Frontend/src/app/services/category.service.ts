import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CategoryDto, CategoryResponse } from '../models/category.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly url = `${environment.apiUrl}/categories`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<CategoryResponse[]> {
    return this.http.get<ApiResponse<CategoryResponse[]>>(this.url).pipe(
      map(res => res.data ?? [])
    );
  }

  add(dto: CategoryDto): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(this.url, dto);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`);
  }
}

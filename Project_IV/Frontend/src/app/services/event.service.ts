import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { EventDto, EventResponse } from '../models/event.model';
import { ApiResponse, PagedResult } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class EventService {
  private readonly url = `${environment.apiUrl}/events`;

  constructor(private http: HttpClient) {}

  getEvents(page = 1, pageSize = 6): Observable<PagedResult<EventResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<ApiResponse<PagedResult<EventResponse>>>(this.url, { params }).pipe(
      map(res => res.data!)
    );
  }

  getEventById(id: string): Observable<EventResponse> {
    return this.http.get<ApiResponse<EventResponse>>(`${this.url}/${id}`).pipe(
      map(res => res.data!)
    );
  }

  createEvent(dto: EventDto): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(this.url, dto);
  }

  updateEvent(id: string, dto: EventDto): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.url}/${id}`, dto);
  }

  deleteEvent(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`);
  }
}

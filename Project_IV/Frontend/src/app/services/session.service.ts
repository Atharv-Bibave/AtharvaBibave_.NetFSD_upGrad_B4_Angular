import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { SessionDto, SessionResponse } from '../models/session.model';
import { ApiResponse, PagedResult } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly url = `${environment.apiUrl}/sessions`;

  constructor(private http: HttpClient) {}

  getByEvent(eventId: string, page = 1, pageSize = 10): Observable<PagedResult<SessionResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<ApiResponse<PagedResult<SessionResponse>>>(
      `${this.url}/by-event/${eventId}`, { params }
    ).pipe(map(res => res.data!));
  }

  getById(id: string): Observable<SessionResponse> {
    return this.http.get<ApiResponse<SessionResponse>>(`${this.url}/${id}`).pipe(
      map(res => res.data!)
    );
  }

  add(dto: SessionDto): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(this.url, this.sanitize(dto));
  }

  update(id: string, dto: SessionDto): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.url}/${id}`, this.sanitize(dto));
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`);
  }

  assignSpeaker(sessionId: string, speakerId: string): Observable<ApiResponse<null>> {
    return this.http.patch<ApiResponse<null>>(
      `${this.url}/${sessionId}/assign-speaker/${speakerId}`, {}
    );
  }

  // Ensure correct types before sending to backend
  private sanitize(dto: any): any {
    const clean: any = {
      eventId:      dto.eventId,
      sessionTitle: dto.sessionTitle,
      sessionStart: dto.sessionStart,
      sessionEnd:   dto.sessionEnd
    };

    // Only include speakerId if it's a real non-empty value
    if (dto.speakerId && dto.speakerId.trim() !== '') {
      clean.speakerId = dto.speakerId;
    }

    // Only include sessionUrl if non-empty
    if (dto.sessionUrl && dto.sessionUrl.trim() !== '') {
      clean.sessionUrl = dto.sessionUrl;
    }

    // Only include description if non-empty
    if (dto.description && dto.description.trim() !== '') {
      clean.description = dto.description;
    }

    return clean;
  }
}

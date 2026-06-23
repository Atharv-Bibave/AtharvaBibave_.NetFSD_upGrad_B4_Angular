import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { ParticipantRegistration } from '../models/participant.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class ParticipantService {
  private readonly url = `${environment.apiUrl}/participants`;

  constructor(private http: HttpClient) {}

  registerForEvent(eventId: string): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(`${this.url}/register/${eventId}`, {});
  }

  getMyEvents(): Observable<ParticipantRegistration[]> {
    return this.http.get<ApiResponse<ParticipantRegistration[]>>(`${this.url}/my-events`).pipe(
      map(res => res.data ?? [])
    );
  }

  getRegistrationById(id: string): Observable<ParticipantRegistration> {
    return this.http.get<ApiResponse<ParticipantRegistration>>(`${this.url}/${id}`).pipe(
      map(res => res.data!)
    );
  }

  markAttendance(registrationId: string, attended: boolean): Observable<ApiResponse<null>> {
    return this.http.patch<ApiResponse<null>>(
      `${this.url}/${registrationId}/attend?attended=${attended}`, {}
    );
  }
}

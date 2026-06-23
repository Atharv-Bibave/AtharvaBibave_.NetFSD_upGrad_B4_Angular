import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { SpeakerDto, SpeakerResponse } from '../models/speaker.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class SpeakerService {
  private readonly url = `${environment.apiUrl}/speakers`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<SpeakerResponse[]> {
    return this.http.get<ApiResponse<SpeakerResponse[]>>(this.url).pipe(
      map(res => res.data ?? [])
    );
  }

  getById(id: string): Observable<SpeakerResponse> {
    return this.http.get<ApiResponse<SpeakerResponse>>(`${this.url}/${id}`).pipe(
      map(res => res.data!)
    );
  }

  add(dto: SpeakerDto): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(this.url, dto);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`);
  }
}

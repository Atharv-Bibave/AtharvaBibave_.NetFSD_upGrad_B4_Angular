export interface SessionDto {
  eventId: string;
  sessionTitle: string;
  speakerId?: string;
  description?: string;
  sessionStart: string;
  sessionEnd: string;
  sessionUrl?: string;
}

export interface SessionResponse {
  sessionId: string;
  eventId: string;
  sessionTitle: string;
  speakerId?: string;
  speakerName?: string;
  description?: string;
  sessionStart: string;
  sessionEnd: string;
  sessionUrl?: string;
}

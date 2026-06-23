export interface EventDto {
  eventName: string;
  categoryId: string;
  eventDate: string;
  description?: string;
  status: string;
}

export interface EventResponse {
  eventId: string;
  eventName: string;
  eventCategory: string;
  categoryId: string;
  eventDate: string;
  description?: string;
  status: string;
}

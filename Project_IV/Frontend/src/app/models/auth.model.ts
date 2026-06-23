export interface RegisterDto {
  emailId: string;
  userName: string;
  password: string;
}

export interface LoginDto {
  emailId: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}

export interface UserResponse {
  emailId: string;
  userName: string;
  role: string;
}

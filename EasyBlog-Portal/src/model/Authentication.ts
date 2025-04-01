export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  authenticated: boolean;
  token: string;
}

export interface APIRegisterRequest {
  email: string;
  fullName: string;
  file: File | null;
  password: string;
}

export interface RegisterRequest extends APIRegisterRequest {
  imageName: string | null;
  confirmPassword: string;
  termAndConditions: boolean;
}

export interface AuthorResponse {
  id: string;
  fullName: string;
  avatar: string;
  postCount?: number;
}

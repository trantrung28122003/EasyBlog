export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  authenticated: boolean;
  token: string;
}

export interface APIRegisterRequest {
  userName: string;
  email: string;
  fullName: string;
  dayOfBirth: string | null;
  file: File | null;
  password: string;
}

export interface RegisterRequest extends APIRegisterRequest {
  imageName: string | null;
  confirmPassword: string;
  termAndConditions: boolean;
}

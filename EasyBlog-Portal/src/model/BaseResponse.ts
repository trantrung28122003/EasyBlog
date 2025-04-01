export interface ApplicationResponse<T> {
  isSuccess: boolean;
  message: string;
  results: T;
  errors?: any;
}

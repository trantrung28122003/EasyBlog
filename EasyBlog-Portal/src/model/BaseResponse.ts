export interface ApplicationResponse<T> {
  code: number;
  result: T;
  message: string;
}

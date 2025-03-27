export interface ApiResponse {
    success: boolean;
    message: string;
    errors: ValidationErrorDetail[];
  }
  
  export interface ApiResponseWithData<T> extends ApiResponse {
    data: T;
  }
  
  export interface ValidationErrorDetail {
    field: string;
    message: string;
  }
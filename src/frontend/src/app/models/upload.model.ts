export interface UploadResponse {
    url: string;
  }
  
  export interface ValidationErrorDetail {
    field?: string;
    message: string;
  }
  
  export interface ApiResponse {
    success: boolean;
    message: string;
    errors: ValidationErrorDetail[];
  }
  
  export interface ApiResponseWithData<T> extends ApiResponse {
    data?: T;
  }
  
  export interface UploadRequest {
    file: File;
  }
  
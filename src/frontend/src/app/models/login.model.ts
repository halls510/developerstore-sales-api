export interface LoginResponse {
    success: boolean;
    message: string;
    errors: { error: string; detail: string }[];
    data: {
      token: string;
      email: string;
      name: string;
      role: string;
    };
  }
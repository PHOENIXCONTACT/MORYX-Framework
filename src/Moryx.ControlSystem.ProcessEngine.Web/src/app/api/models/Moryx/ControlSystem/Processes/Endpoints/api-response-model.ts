import { ApiError } from "./api-error-model";

export interface ApiResponse<T> {
  data?: T;
  errors?: Array<ApiError>;
  succeeded?: boolean;
  timestamp?: Date;
}

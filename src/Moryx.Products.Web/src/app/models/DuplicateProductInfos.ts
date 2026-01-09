import { ProductModel } from '../api/models';

export interface DuplicateProductInfos {
  product?: ProductModel;
  identifier?: string;
  revision?: number;
}

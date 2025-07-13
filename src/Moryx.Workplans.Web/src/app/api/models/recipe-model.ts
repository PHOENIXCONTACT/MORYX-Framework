/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { RecipeClassificationModel } from './recipe-classification-model';
import { RecipeState } from './recipe-state';
import { WorkplanModel } from './workplan-model';
export interface RecipeModel {
  classification?: RecipeClassificationModel;
  id?: number;
  isClone?: boolean;
  name?: null | string;
  properties?: Entry;
  revision?: number;
  state?: RecipeState;
  type?: null | string;
  /** @deprecated */workplanId?: number;
  workplanModel?: WorkplanModel;
}

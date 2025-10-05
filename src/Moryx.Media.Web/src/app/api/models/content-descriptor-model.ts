/* tslint:disable */
/* eslint-disable */
import { VariantDescriptor } from './variant-descriptor';
export interface ContentDescriptorModel {
  id?: string;
  master?: VariantDescriptor;
  name?: null | string;
  variants?: null | Array<VariantDescriptor>;
}

/* tslint:disable */
/* eslint-disable */
import { PreviewState } from './preview-state';
export interface PreviewDescriptor {
  extension?: null | string;
  fileHash?: null | string;
  mimeType?: null | string;
  size?: number;
  state?: PreviewState;
}

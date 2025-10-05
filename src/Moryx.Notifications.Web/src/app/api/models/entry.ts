/* tslint:disable */
/* eslint-disable */
import { EntryValidation } from './entry-validation';
import { EntryValue } from './entry-value';
export interface Entry {
  description?: null | string;
  displayName?: null | string;
  identifier?: null | string;
  prototypes?: null | Array<Entry>;
  subEntries?: null | Array<Entry>;
  validation?: EntryValidation;
  value: EntryValue;
}

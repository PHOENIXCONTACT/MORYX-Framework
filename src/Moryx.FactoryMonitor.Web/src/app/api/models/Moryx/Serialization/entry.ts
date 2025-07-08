/* tslint:disable */
/* eslint-disable */
import { EntryValidation as MoryxSerializationEntryValidation } from '../../Moryx/Serialization/entry-validation';
import { EntryValue as MoryxSerializationEntryValue } from '../../Moryx/Serialization/entry-value';
export interface Entry {
  description?: null | string;
  displayName?: null | string;
  identifier?: null | string;
  prototypes?: null | Array<Entry>;
  subEntries?: null | Array<Entry>;
  validation?: MoryxSerializationEntryValidation;
  value?: MoryxSerializationEntryValue;
}

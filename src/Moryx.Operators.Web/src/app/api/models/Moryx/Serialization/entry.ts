/* tslint:disable */
/* eslint-disable */
import { EntryValidation as MoryxSerializationEntryValidation } from '../../../models/Moryx/Serialization/entry-validation';
import { EntryValue as MoryxSerializationEntryValue } from '../../../models/Moryx/Serialization/entry-value';
export interface Entry {
  description?: string | null;
  displayName?: string | null;
  identifier?: string | null;
  prototypes?: Array<Entry> | null;
  subEntries?: Array<Entry> | null;
  validation?: MoryxSerializationEntryValidation;
  value?: MoryxSerializationEntryValue;
}

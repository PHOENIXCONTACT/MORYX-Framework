/* tslint:disable */
/* eslint-disable */
import { EntryUnitType as MoryxSerializationEntryUnitType } from '../../../models/Moryx/Serialization/entry-unit-type';
import { EntryValueType as MoryxSerializationEntryValueType } from '../../../models/Moryx/Serialization/entry-value-type';
export interface EntryValue {
  current?: string | null;
  default?: string | null;
  isReadOnly?: boolean;
  possible?: Array<string> | null;
  type?: MoryxSerializationEntryValueType;
  unitType?: MoryxSerializationEntryUnitType;
}

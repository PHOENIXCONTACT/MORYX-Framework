/* tslint:disable */
/* eslint-disable */
import { EntryUnitType as MoryxSerializationEntryUnitType } from '../../Moryx/Serialization/entry-unit-type';
import { EntryValueType as MoryxSerializationEntryValueType } from '../../Moryx/Serialization/entry-value-type';
export interface EntryValue {
  current?: null | string;
  default?: null | string;
  isReadOnly?: boolean;
  possible?: null | Array<string>;
  type?: MoryxSerializationEntryValueType;
  unitType?: MoryxSerializationEntryUnitType;
}

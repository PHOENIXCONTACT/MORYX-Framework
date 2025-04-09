# Migration from MORYX Framework v8 to v10

## Removal of ProductFileEntity

The `ProductFileEntity` was not used in the product-model. There was no api to access it in a simple way. With the implementation of the [`FileSystem`](https://github.com/PHOENIXCONTACT/MORYX-Framework/pull/517) it is also not required anymore.

## Renamings and Typo-Fixes

- ResourceRelationType.CurrentExchangablePart -> ResourceRelationType.CurrentExchangeablePart
- ResourceRelationType.PossibleExchangablePart -> ResourceRelationType.PossibleExchangeablePart

/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { lastValueFrom, Observable } from 'rxjs';

declare module 'rxjs' {
  interface Observable<T> {
    toAsync: () => Promise<T>;
  }
}

Observable.prototype.toAsync = function () {
  return lastValueFrom(this);
};


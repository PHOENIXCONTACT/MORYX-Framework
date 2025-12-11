import { lastValueFrom, Observable } from 'rxjs';

declare module 'rxjs' {
  interface Observable<T> {
    toAsync: () => Promise<T>;
  }
}

Observable.prototype.toAsync = function () {
  return lastValueFrom(this);
};

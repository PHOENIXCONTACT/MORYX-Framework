interface Array<T> {
    clone(): Array<T>;
    remove(item: T): void;
    distinct(): Array<T>;
    any(predicate: (item: T) => boolean): boolean;
    skip(amount: number): T[];
    take(amount: number): T[];
  }
  
  Array.prototype.clone = function <T>(this: T[]): T[] {
    return this.slice();
  };
  
  Array.prototype.remove = function <T>(this: T[], item: T): void {
    this.splice(this.indexOf(item), 1);
  };
  
  Array.prototype.distinct = function <T>(this: T[]): T[] {
    return this.filter((item, index, self) => self.indexOf(item) === index);
  };
  
  Array.prototype.any = function <T>(
    this: T[],
    predicate: (item: T) => boolean
  ): boolean {
    return this.find(predicate) != undefined;
  };
  
  Array.prototype.skip = function <T>(amount: number): T[] {
    return this.slice(amount);
  };
  
  Array.prototype.take = function <T>(amount: number): T[] {
    return this.slice(0, amount);
  };
  
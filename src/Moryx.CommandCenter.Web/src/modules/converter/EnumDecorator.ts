type DecoratedEnum<T extends Record<string, string>> = {
  [K in keyof T as K | Lowercase<K & string>]: T[K];
};

const decoratedEnums = new WeakMap<object, Record<string, string>>();

function createDecoratedEnum<T extends Record<string, string>>(enumObj: T): DecoratedEnum<T> {
  const result: any = {} as DecoratedEnum<T>;

  if (decoratedEnums.has(enumObj)){
    return decoratedEnums.get(enumObj) as DecoratedEnum<T>;
  }

  (Object.keys(enumObj) as (keyof T)[]).forEach((key) => {
    const value = enumObj[key];
    const lowerKey = key.toString().toLowerCase() as Lowercase<typeof key & string>;
    result[key] = value;
    result[lowerKey] = value;
  });

  decoratedEnums.set(enumObj, result);

  return result;
}

function getEnumValueForKey<T extends Record<string, string>>(
  enumObj: T,
  key: string
): T[keyof T] | undefined {
  const decorated = createDecoratedEnum(enumObj);
  return decorated[key.toLowerCase()];
}

export { getEnumValueForKey, createDecoratedEnum };

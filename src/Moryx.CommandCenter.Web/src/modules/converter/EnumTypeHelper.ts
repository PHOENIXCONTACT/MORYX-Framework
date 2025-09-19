function getEnumTypeValue<T>(enumType: T, key: string): T[keyof T] | undefined {
  return Object.values(enumType).find(
    (v) => v.toLowerCase() === key.toLowerCase()
  ) as T[keyof T] | undefined;
}

export { getEnumTypeValue };

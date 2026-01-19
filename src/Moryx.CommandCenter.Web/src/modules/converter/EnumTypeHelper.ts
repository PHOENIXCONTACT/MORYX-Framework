/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

function getEnumTypeValue<T>(enumType: T, key: string): T[keyof T] | undefined {
  return Object.values(enumType).find(
    (v) => v.toLowerCase() === key.toLowerCase()
  ) as T[keyof T] | undefined;
}

export { getEnumTypeValue };


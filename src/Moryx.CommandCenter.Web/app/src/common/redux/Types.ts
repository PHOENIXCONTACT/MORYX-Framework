/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

interface Action<T> {
    type: string;
    payload?: T;
  }

export type ActionType<T> = Action<T>;

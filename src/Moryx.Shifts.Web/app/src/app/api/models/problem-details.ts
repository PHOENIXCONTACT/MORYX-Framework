/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
export interface ProblemDetails {
  detail?: string | null;
  instance?: string | null;
  status?: number | null;
  title?: string | null;
  type?: string | null;

  [key: string]: any | number | null | string | null | undefined;
}


/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { NotificationModel, Severity } from '@api/models';

declare global {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  interface Array<T> {
    sortBySeverity(): NotificationModel[];
  }
}

Array.prototype.sortBySeverity = function (): NotificationModel[] {
  const severityOrder = Object.values(Severity);
  return this.sort((n1, n2) =>
    severityOrder.indexOf(n2.severity ?? Severity.Info) -
    severityOrder.indexOf(n1.severity ?? Severity.Info)
  );
};


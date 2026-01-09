import { Severity } from '../api/models';
import { NotificationModel } from '../api/models/notification-model';

declare global {
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

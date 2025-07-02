import { NotificationModel, Severity } from "./api/models";

  export function getIcon(notification: NotificationModel): string {
    switch (notification.severity) {
      case Severity.Info:
        return 'info_outline';
      case Severity.Warning:
        return 'warning_amber';
      case Severity.Error:
        return 'error_outline';
      case Severity.Fatal:
        return 'new_releases';
      default:
        return '';
    }
  }
import { ModuleNotificationType } from "./ModuleNotificationType";
import SerializableException from "./SerializableException";

export default class NotificationModel {
    public Timestamp: Date;
    public Important: boolean;
    public Exception: SerializableException;
    public NotificationType: ModuleNotificationType;
}

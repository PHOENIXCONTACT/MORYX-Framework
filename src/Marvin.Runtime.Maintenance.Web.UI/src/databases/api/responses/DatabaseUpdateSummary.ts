import DatabaseUpdate from "../../models/DatabaseUpdate";

export default class DatabaseUpdateSummary {
    public WasUpdated: boolean;
    public ExecutedUpdates: DatabaseUpdate[];
}

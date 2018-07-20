import DatabaseConfigModel from "../../models/DatabaseConfigModel";
import ScriptModel from "../../models/ScriptModel";

export default class ExecuteScriptRequest
{
    Config : DatabaseConfigModel;
    Script : ScriptModel;
}
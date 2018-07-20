import DatabaseConfigModel from "../../models/DatabaseConfigModel";
import SetupModel from "../../models/SetupModel";

export default interface ExecuteSetupRequest
{
    Config : DatabaseConfigModel;
    Setup : SetupModel;
}
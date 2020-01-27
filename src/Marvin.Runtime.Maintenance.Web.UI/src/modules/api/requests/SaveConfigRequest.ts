import Config from '../../models/Config';
import { ConfigUpdateMode } from '../../models/ConfigUpdateMode';

export default class SaveConfigRequest
{
    Config: Config;
    UpdateMode: ConfigUpdateMode;
}

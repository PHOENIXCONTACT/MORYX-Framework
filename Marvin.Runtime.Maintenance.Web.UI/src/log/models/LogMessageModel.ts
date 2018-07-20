import LoggerModel from './LoggerModel'
import { LogLevel } from './LogLevel'

export default class LogMessageModel
{
    Logger : LoggerModel;
    ClassName : string;
    LogLevel: LogLevel;
    Message : string;
    Timestamp : Date;
}

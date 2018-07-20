export default class SerializableException
{
    ExceptionTypeName : string;
    Message : string;
    StackTrace : string;
    InnerException : SerializableException;
}
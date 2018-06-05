namespace Marvin.DependentTestModule
{
    public interface ISimpleHelloWorldWcfSvcMgr
    {
        string Hello(string name);
        string Throw(string name);
    }
}
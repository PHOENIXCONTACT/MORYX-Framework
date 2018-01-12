using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    [ModelScript(TestModelConstants.Name)]
    public class SomeCoolScript : IModelScript
    {
        public string Name => "Some cool name";

        public bool IsCreationScript => true;

        public string GetSql()
        {
            return "INSERT INTO \"HouseEntity\"(\"Name\") VALUES ('From CreationScript');";
        }
    }
}

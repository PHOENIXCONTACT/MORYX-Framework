using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    [ModelScript(TestModelConstants.Namespace)]
    public class SomeCoolScript : IModelScript
    {
        public string Name => "Some cool name";

        public bool IsCreationScript => true;

        public string GetSql()
        {
            return $"INSERT INTO {TestModelConstants.Schema.ToLower()}.\"{nameof(HouseEntity)}\"(\"{nameof(HouseEntity.Name)}\") " +
                   "VALUES ('From CreationScript');";
        }
    }
}

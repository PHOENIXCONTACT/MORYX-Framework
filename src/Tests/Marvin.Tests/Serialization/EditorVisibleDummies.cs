using Marvin.Serialization;

namespace Marvin.Tests
{
    public class EditorVisibleMixed
    {
        [EditorVisible]
        public string Property1 { get; set; } = "123456";
        [EditorVisible]
        public bool Property2 { get; set; } = true;
        public int Property3 { get; set; } = 98;

        [EditorVisible]
        public bool Method1()
        {
            return true;
        }

        public string Method2()
        {
            return "1234";
        }
    }

    public class NoEditorVisibleSet
    {
        public string Property1 { get; set; } = "123456";
        public bool Property2 { get; set; } = true;
        public int Property3 { get; set; } = 98;

        public bool Method1()
        {
            return true;
        }

        public string Method2()
        {
            return "1234";
        }
    }
}

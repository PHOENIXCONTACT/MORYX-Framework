using System.Globalization;

namespace Marvin.PlatformTools.Tests.Configuration
{
    public class FaultyConfig
    {
        public static string Content()
        {
            var content =
@"{{
  ""DummyString"": ""{0}"",
  ""Child"": {{
    ""DummyDouble"": {1}
  }},
  ""ConfigState"": ""Generated""
}}";
            return string.Format(content, ModifiedValues.Text, ModifiedValues.Decimal.ToString(CultureInfo.InvariantCulture));
        }
    }
}
using System.IO;
using System.ServiceModel.Web;
using System.Text;

namespace Marvin.Tools.Wcf.FileSystem
{
    public class PolicyRetriever : IPolicyRetriever
    {
        public Stream GetSilverlightPolicy()
        {
            return XmlStringToStream(GetSilverlightPolicyString());
        }

        public Stream GetFlashPolicy()
        {
            return XmlStringToStream(GetFlashPolicyString());
        }

        private Stream XmlStringToStream(string xmlString)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            return new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
        }

        public static string GetFlashPolicyString()
        {
            return @"<?xml version=""1.0""?>
<!DOCTYPE cross-domain-policy SYSTEM ""http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd"">
<cross-domain-policy>
  <allow-http-request-headers-from domain=""*"" headers=""*""/>
</cross-domain-policy>";
        }

        public static string GetSilverlightPolicyString()
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<access-policy>
     <cross-domain-access>
         <policy>
             <allow-from http-request-headers=""*"">
                  <domain uri=""http://*"" />
                  <domain uri=""https://*"" />
             </allow-from>
             <grant-to>
                 <resource path=""/"" include-subpaths=""true""/>
             </grant-to>
         </policy>
     </cross-domain-access>
</access-policy>";
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;

namespace Marvin.Tools.Wcf.FileSystem
{
    public class SimpleStreamingServiceBase
    {
        public SimpleStreamingServiceBase()
        {
            WebRoot = "%TEMP%\\";
            IndexFile = "index.html";
            SecondaryWebRoots = new List<string>();
        }

        public string WebRoot { get; set; }

        public string IndexFile { get; set; }

        public List<string> SecondaryWebRoots { get; private set; }

        public virtual Stream StreamFile()
        {
            Stream stream = null;
            string relativePath;
            var segments = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.Segments;

            if (segments.Length > 2)
            {
                relativePath = string.Join("", segments, 2, segments.Length - 2).Replace("/", "\\");
            }
            else
            {
                relativePath = IndexFile;
            }

            relativePath = HttpUtility.UrlDecode(relativePath);

            var fullPathsOnServer = BuildAbsoluteLocalPaths(relativePath);
            fullPathsOnServer.Insert(0, BuildAbsoluteLocalPath(relativePath));

            foreach (var fullPathOnServer in fullPathsOnServer)
            {
                var localFullPathOnServer = fullPathOnServer;

                if (Directory.Exists(localFullPathOnServer))
                {
                    localFullPathOnServer = fullPathOnServer + "\\" + IndexFile;
                }

                if (File.Exists(localFullPathOnServer))
                {
                    var mimeType = GetMimeType(localFullPathOnServer);

                    WebOperationContext.Current.OutgoingResponse.ContentType = mimeType;
                    stream = new FileStream(localFullPathOnServer, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    break;
                }
            }

            if(stream == null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            }


            return stream;
        }

        protected string BuildAbsoluteLocalPath(string webServerPath)
        {
            return BuildAbsoluteLocalPath(WebRoot, webServerPath);
        }

        protected string BuildAbsoluteLocalPath(string root, string webServerPath)
        {
            return Path.Combine(root, webServerPath.Replace('/', '\\'));
        }

        protected List<string> BuildAbsoluteLocalPaths(string webServerPath)
        {
            return SecondaryWebRoots.Select(secondaryWebRoot => BuildAbsoluteLocalPath(secondaryWebRoot, webServerPath)).ToList();
        }

        protected static string GetMimeType(string fileName)
        {
            var mimeType = "application/unknown";

            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var ext = extension.ToLower();
                var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

                if (regKey != null && regKey.GetValue("Content Type") != null)
                {
                    mimeType = regKey.GetValue("Content Type").ToString();
                }

                // This is a special handling (=bloody workaround) for debugging Silverlight apps in a cross-domain context
                // Requirement: For Silverlight .xap files we should always return "application/x-silverlight-app"
                // Problem: Win Registry does not - by default - provide a default setting  
                // Either we make sure that the registry entries are present or we choose to live with this workaround:
                // <workaround>
                if (ext == ".xap")
                {
                    mimeType = "application/x-silverlight-app";
                }
                // </workaround>
            }

            return mimeType;
        } 
    }
}

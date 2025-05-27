using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceWebRole
{
    public class Service : IService
    {
        public void Encode(string name, string content)
        {
            if (name is null || content is null)
            {
                return;
            }
            var contentBytes = Encoding.UTF8.GetBytes(content);
            BlobService.UploadBlob("upload", name, contentBytes);
            QueueService.AddMessage("encrypt", name);
        }

        public string Fetch(string name)
        {
            if (name is null)
            {
                return null;
            }
            var contentBytes = BlobService.DownloadBlob("result", name);
            if (contentBytes == null || contentBytes.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(contentBytes);
        }
    }
}

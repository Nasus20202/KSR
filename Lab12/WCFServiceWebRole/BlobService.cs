using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Azure.Storage.Blobs;

namespace WCFServiceWebRole
{
	public class BlobService
	{
        private static string _connectionString = "UseDevelopmentStorage=true";

        private static BlobContainerClient GetContainerClient(string name)
        {
            var client = new BlobContainerClient(_connectionString, name);
            client.CreateIfNotExists();
            return client;
        }

        public static void UploadBlob(string containerName, string blobName, byte[] content)
        {
            var containerClient = GetContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            using (var stream = new System.IO.MemoryStream(content))
            {
                blobClient.Upload(stream, overwrite: true);
            }
        }

        public static byte[] DownloadBlob(string containerName, string blobName)
        {
            var containerClient = GetContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!blobClient.Exists())
            {
                return null;
            }

            var response = blobClient.Download();
            using(var reader = new StreamReader(response.Value.Content))
            {
                var content = reader.ReadToEnd();
                return System.Text.Encoding.UTF8.GetBytes(content);
            }
        }
    }
}
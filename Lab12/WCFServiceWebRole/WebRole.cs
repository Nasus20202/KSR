using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WCFServiceWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            return base.OnStart();
        }

        public override void Run()
        {
            var random = new Random();
            while (true)
            {
                var message = QueueService.GetMessage("encrypt");
                if (message is null)
                {
                    Thread.Sleep(100);
                    continue;
                }

                try
                {
                    var contentBytes = BlobService.DownloadBlob("upload", message.MessageText);
                    var content = System.Text.Encoding.UTF8.GetString(contentBytes);

                    if (random.Next(3) == 0)
                    {
                        throw new Exception("Simulated failure");
                    }

                    string encrypted = EncryptionService.Encrypt(content);
                    BlobService.UploadBlob("result", message.MessageText, System.Text.Encoding.UTF8.GetBytes(encrypted));
                    QueueService.DeleteMessage("encrypt", message.MessageId, message.PopReceipt);

                    Trace.WriteLine($"Processed message: {message.MessageId} with content: {content} encrypted to: {encrypted}");
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Error processing message: {message.MessageId} {e.Message}");
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }
    } 
}

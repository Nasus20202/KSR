using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zadanie5
{
    [ServiceContract]
    public interface ISerwis
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Dodaj/{a}/{b}")]
        int Dodaj(string a, string b);
    }
    class Program
    {
        static void Main(string[] args)
        {
            var channelFactory = new ChannelFactory<ISerwis>(new WebHttpBinding(), new EndpointAddress("http://localhost:30703/Serwis.svc/"));
            channelFactory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            var channel = channelFactory.CreateChannel();
            Console.WriteLine(channel.Dodaj("123", "456"));
            channelFactory.Close();
        }
    }
}

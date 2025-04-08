using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2
{
    [ServiceContract]
    interface ISerwis
    {
        [OperationContract]
        string ScalNapisy(string a, string b);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint("soap.udp://127.0.0.1:30703"));
            var endpoints = discoveryClient.Find(new FindCriteria(typeof(ISerwis))).Endpoints;

            if (endpoints.Count == 0)
                return;
               
            var endpoint = endpoints.First().Address;
            var channel = ChannelFactory<ISerwis>.CreateChannel(new NetNamedPipeBinding(), endpoint);
            Console.WriteLine(channel.ScalNapisy("abc", "123"));

            ((IDisposable)channel).Dispose();
        }
    }
}

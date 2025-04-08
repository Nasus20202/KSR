using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie1
{
    [ServiceContract]
    interface ISerwis
    {
        [OperationContract]
        string ScalNapisy(string a, string b);
    }

    class Serwis : ISerwis
    {
        public string ScalNapisy(string a, string b)
        {
            return a + b;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Serwis));
            host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint("soap.udp://localhost:30703"));
            host.AddServiceEndpoint(typeof(ISerwis), new NetNamedPipeBinding(), "net.pipe://localhost/Serwis");
            host.Open();
            Console.WriteLine("Serwis uruchomiony. Naciśnij dowolny klawisz, aby zakończyć.");
            Console.ReadKey();
            host.Close();
        }
    }
}

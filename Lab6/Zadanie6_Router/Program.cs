using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Routing;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie6_Router
{
    class Program
    {
        static void Main(string[] args)
        {
            const string firstServiceAddress = "net.pipe://localhost/Zadanie6_SerwisPierwszy",
                secondServiceAddress = "net.pipe://localhost/Zadanie6_SerwisDrugi",
                routingAddress = "net.pipe://localhost/Zadanie6";

            var host = new ServiceHost(typeof(RoutingService));
            host.AddServiceEndpoint(typeof(IRequestReplyRouter), new NetNamedPipeBinding(), routingAddress);

            var contract = ContractDescription.GetContract(typeof(IRequestReplyRouter));
            var firstClient = new ServiceEndpoint(contract, new NetNamedPipeBinding(), new EndpointAddress(firstServiceAddress));
            var secondClient = new ServiceEndpoint(contract, new NetNamedPipeBinding(), new EndpointAddress(secondServiceAddress));

            var endpoints = new List<ServiceEndpoint> { firstClient, secondClient };
            var routingConfiguration = new RoutingConfiguration();
            routingConfiguration.FilterTable.Add(new MatchAllMessageFilter(), endpoints);
            host.Description.Behaviors.Add(new RoutingBehavior(routingConfiguration));

            host.Open();
            Console.WriteLine("Router: Kliknij coś aby zakończyć");
            Console.ReadKey();
            host.Close();
        }
    }
}

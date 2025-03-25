using KSR_WCF2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace KSR_WCF2
{
    [ServiceContract]
    public interface IZadanie2
    {
        [OperationContract]
        string Test(string arg);
    }

    // Zadanie 7
    [ServiceContract]
    public interface IZadanie7
    {
        [OperationContract]
        [FaultContract(typeof(Wyjatek7))]
        void RzucWyjatek7(string a, int b);
    }

    [DataContract] // Dodaj System.Runtime.Serialization
    public class Wyjatek7
    {
        [DataMember]
        public string opis;
        [DataMember]
        public string a;
        [DataMember]
        public int b;
    }
    // Koniec zadania 7
}

namespace Zadanie2
{
    public class Serwer : IZadanie2, /* Zadanie 7 */ IZadanie7 
    {

        public string Test(string arg)
        {
            var msg = $"Test: {arg}";
            Console.WriteLine(msg);
            return msg;
        }

        // Zadanie 7
        public void RzucWyjatek7(string a, int b)
        {
            throw new FaultException<Wyjatek7>(new Wyjatek7
            {
                opis = $"Wyjątek z zadania 7 (a={a}, b={b})",
                a = a,
                b = b
            });
        }
        // Koniec zadania 7
    }

    class Program
    {
        static void Main(string[] args)
        {
            // netsh http add urlacl url=http://+:1100/ user=<current_user>
            var host = new ServiceHost(typeof(Serwer), new Uri[] { new Uri("http://localhost:1100"),
                // Zadanie 4
                new Uri("net.tcp://127.0.0.1:55765")
                // Koniec zadania 4
            });

            // Zadanie 3
            var behavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(behavior);
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "net.pipe://localhost/metadane");
            // Dodaj net.pipe://localhost/metadane w Project -> Add Service Reference po uruchomieniu bez debugowania
            // Koniec zadania 3

            host.AddServiceEndpoint(typeof(IZadanie2), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf1-zad2");

            // Zadanie 4
            host.AddServiceEndpoint(typeof(IZadanie2), new NetTcpBinding(), "net.tcp://127.0.0.1:55765/");
            // Koniec zadania 4

            // Zadanie 7
            host.AddServiceEndpoint(typeof(IZadanie7), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf1-zad7");
            // Koniec zadania 7

            host.Open();
            Console.WriteLine("Serwer uruchomiony, naciśnij dowolny klawisz, aby zakończyć...");
            Console.ReadKey();
            host.Close();
        }
    }
}

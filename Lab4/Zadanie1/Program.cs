using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using KSR_WCF1;

// Potrzebne jeśli nie dodajemy referencji do KSR_WCF1.dll
/*namespace KSR_WCF1
{
    [ServiceContract]
    public interface IZadanie1
    {
        [OperationContract]
        string Test(string arg);

        [OperationContract]
        [FaultContract(typeof(Wyjatek))]
        void RzucWyjatek(bool czy_rzucic);

        [OperationContract]
        string OtoMagia(string magia);
    }

    [DataContract]
    public class Wyjatek
    {
        [DataMember]
        public string opis;

        [DataMember]
        public string magia { get; set; }
    }
}*/

namespace Zadanie1
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ChannelFactory<IZadanie1>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/ksr-wcf1-test"));
            var client = factory.CreateChannel();

            Console.WriteLine(client.Test("Hello World!"));

            // Zadanie 5
            try
            {
                client.RzucWyjatek(true);
            }
            catch (FaultException<Wyjatek> ex)
            {
                Console.WriteLine(ex.Detail.opis);
                Console.WriteLine(client.OtoMagia(ex.Detail.magia));
            }
            // Koniec zadania 5

            ((IDisposable)client).Dispose();
            factory.Close();
        }
    }
}

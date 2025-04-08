using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie6_SerwisDrugi
{
    [ServiceContract]
    interface ISerwis
    {
        [OperationContract]
        int Dodaj(int a, int b);
    }

    class Serwis : ISerwis
    {
        public int Dodaj(int a, int b)
        {
            Console.WriteLine($"SerwisDrugi: {a}+{b}={a+b}");
            return a + b;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Serwis));
            host.AddServiceEndpoint(typeof(ISerwis), new NetNamedPipeBinding(), "net.pipe://localhost/Zadanie6_SerwisDrugi");
            host.Open();
            Console.WriteLine("SerwisDrugi: Kliknij coś aby zakończyć");
            Console.ReadKey();
            host.Close();
        }
    }
}

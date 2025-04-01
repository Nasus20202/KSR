using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using KSR_WCF2;

namespace Zadanie3
{
    class Program
    {
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
        class Serwer : IZadanie3, IZadanie4
        {
            private int licznik = 0;

            public void TestujZwrotny()
            {
                var callbackChannel = OperationContext.Current.GetCallbackChannel<IZadanie3Zwrotny>();
                foreach (var x in Enumerable.Range(0, 31))
                {
                    callbackChannel.WolanieZwrotne(x, x * x * x - x * x);
                }
            }

            public int Dodaj(int v)
            {
                licznik += v;
                return licznik;
            }

            public void Ustaw(int v)
            {
                licznik = v;
            }
        }

        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Serwer), new Uri[] { new Uri("http://localhost:1100") });
            host.AddServiceEndpoint(typeof(IZadanie3), new
                NetNamedPipeBinding(),
                "net.pipe://localhost/ksr-wcf2-zad3");
            host.AddServiceEndpoint(typeof(IZadanie4), new
                NetNamedPipeBinding(),
                "net.pipe://localhost/ksr-wcf2-zad4");

            host.Open();
            Console.WriteLine("Press any key to close");
            Console.Read();
            host.Close();
        }
    }
}

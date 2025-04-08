using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadanie6_Klient
{
    [ServiceContract]
    interface ISerwis
    {
        [OperationContract]
        int Dodaj(int a, int b);
    }
    class Program
    {
        static void Main(string[] args)
        {
            int retryCount = 0;
            while (retryCount < 10)
            {
                try
                {
                    var factoryChannel = new ChannelFactory<ISerwis>(new NetNamedPipeBinding(), "net.pipe://localhost/Zadanie6");
                    var channel = factoryChannel.CreateChannel();

                    var random = new Random();
                    while (true)
                    {
                        int a = random.Next(1, 100),
                            b = random.Next(1, 100);
                        Console.WriteLine($"Klient: {a} + {b} = {channel.Dodaj(a, b)}");
                        Thread.Sleep(1000);
                    }
                }
                catch
                {
                    Console.WriteLine("Nie można znaleźć usługi. Ponawiam próbę...");
                    Thread.Sleep(1000);
                    retryCount++;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Zadanie1.Server;

namespace Zadanie1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client1 = new Zadanie1Client();
            var result1 = client1.DlugieObliczeniaAsync();
            foreach (var x in Enumerable.Range(0, 21))
            {
                client1.Szybciej(x, 3 * x * x - 2 * x);
            }
            Console.WriteLine(await result1);

            var client2 = new Zadanie2Client(new InstanceContext(new Zadanie2Callback()));
            await client2.PodajZadaniaAsync();
            Console.ReadKey();
        }

        class Zadanie2Callback : IZadanie2Callback
        {
            public void Zadanie([MessageParameter(Name = "zadanie")] string zadanie1, int pkt, bool zaliczone)
            {
                Console.WriteLine($"zadanie1={zadanie1}, pkt={pkt}, zaliczone={zaliczone}");
            }
        }
    }
}

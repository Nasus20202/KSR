using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Zadanie7.Serwis;

namespace Zadanie7
{
    class Program
    {
        static void Main(string[] args)
        {
            var client5 = new Zadanie5Client();
            var client6 = new Zadanie6Client(new InstanceContext(new Zadanie6Callback()));

            Console.WriteLine(client5.ScalNapisy("Zadanie", " 7"));
            client6.Dodaj(123, 456);

            Console.ReadKey();
        }

        class Zadanie6Callback : IZadanie6Callback
        {
            public void Wynik(int wyn)
            {
                Console.WriteLine($"wyn={wyn}");
            }
        }
    }
}

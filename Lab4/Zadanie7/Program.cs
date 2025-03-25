using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Zadanie7.ServiceReference1;

// Dodaj serwer przez Project -> Add Service Reference -> net.pipe://localhost/metadane

namespace Zadanie7
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ServiceReference1.Zadanie7Client();
            try
            {
                client.RzucWyjatek7("test", 123);
            } catch (FaultException<ServiceReference1.Wyjatek7> ex)
            {
                Console.WriteLine(ex.Detail.opis);
            }   
        }
    }
}

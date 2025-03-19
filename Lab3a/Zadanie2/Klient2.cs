using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2
{
    class Klient2
    {
        static void Main(string[] args)
        {
            var type = Type.GetTypeFromProgID("KSR20.COM3Klasa.1");
            var comObject = Activator.CreateInstance(type);

            var invokeArgs = new object[] { "Lab 3 - Zadanie 2" };
            type.InvokeMember("Test", System.Reflection.BindingFlags.InvokeMethod, null, comObject, invokeArgs);
        }
    }
}

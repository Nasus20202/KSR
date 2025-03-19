using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using klasacnet;

/*
 *  tlbimp .\klasac.tlb /out:klasacnet.dll
 */

namespace Zadanie4
{
    class Klient4
    {
        static void Main(string[] args)
        {
            IKlasa klasa = new Klasa();
            klasa.Test("Lab 3 - Zadanie 4");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
﻿using KSR_WCF2;

namespace Zadanie5
{
    // ./test.exe http://localhost:50683/Serwis.svc/zadanie5 http://localhost:50683/Serwis.svc/zadanie6
    public class Serwis : IZadanie5, IZadanie6
    {
        public string ScalNapisy(string a, string b)
        {
            return a + b;
        }

        public void Dodaj(int a, int b)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IZadanie6Zwrotny>();
            channel.Wynik(a + b);
        }
    }
}
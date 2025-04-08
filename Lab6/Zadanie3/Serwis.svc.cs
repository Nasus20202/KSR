using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;

namespace Zadanie3
{
    public class Serwis : ISerwis
    {
        public int Dodaj(string a, string b)
        {
            return int.Parse(a) + int.Parse(b);
        }

        public XmlDocument Index()
        {
            var document = new XmlDocument();
            document.Load(HttpContext.Current.Server.MapPath("~/index.xhtml"));
            return document;
        }

        public Stream Script()
        {
            return File.OpenRead(HttpContext.Current.Server.MapPath("~/scripts.js"));
        }
    }
}

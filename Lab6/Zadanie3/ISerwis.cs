using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace Zadanie3
{
	[ServiceContract]
	public interface ISerwis
	{
		[OperationContract]
		[WebInvoke(UriTemplate = "Dodaj/{a}/{b}")]
		int Dodaj(string a, string b);

        [OperationContract]
		[WebGet(UriTemplate = "index.html")]
		[XmlSerializerFormat]
		XmlDocument Index();

		[OperationContract]
		[WebGet(UriTemplate = "scripts.js")]
		Stream Script();
    }
}

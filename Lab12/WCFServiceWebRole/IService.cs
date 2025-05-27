using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceWebRole
{
	[ServiceContract]
	public interface IService
	{

		[OperationContract]
		void Encode(string name, string content);

		[OperationContract]
		string Fetch(string name);
	}
}

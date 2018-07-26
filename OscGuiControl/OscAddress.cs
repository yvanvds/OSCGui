using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscGuiControl
{
	public class OscAddressList
	{
		public List<OscAddress> List = new List<OscAddress>();
		int Count => List.Count;
	}

	public class OscAddress
	{
		public string Address => OscTree.RouteToAddress(route);
		public List<string> Route => route;
		public void CreateRoute(List<string> parentroute)
		{
			route = new List<string>(parentroute);
			route.Add(UID);
		}

		public void SetJsonRoute(JArray route)
		{
			this.route = new List<string>();
			foreach(string elm in route)
			{
				this.route.Add(elm);
			}
			UID = Name = this.route.Last();
		}

		public OscAddress Self { get => this; }

		internal string UID;
		internal string Name;
		private List<String> route;
		
	}
}

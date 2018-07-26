using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscGuiControl
{
	public class OscObjectRoutes
	{
		private Dictionary<string, Action<object[]>> routes = new Dictionary<string, Action<Object[]>>();
		public List<string> List{ get => routes.Keys.ToList(); }

		public void Add(string route, Action<object[]> action)
		{
			routes.Add(route, action);
		}

		public bool Deliver(string route, object[] arguments)
		{
			if(routes.ContainsKey(route))
			{
				routes[route](arguments);
				return true;
			}
			return false;
		}

		public LinkedList<string> AddressToRoute(List<string> address, int pos)
		{
			var result = new LinkedList<string>();
			if (address.Count <= pos) return result;

			if(routes.ContainsKey(address[pos])) {
				result.AddLast(address[pos]);
			}

			return result;
		}
	}
}

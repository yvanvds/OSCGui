using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscGuiControl
{
	public class OscArgs : EventArgs
	{
		public string Address { get; set; }
		public object[] Arguments { get; set; }

		public OscArgs(string address, object[] arguments)
		{
			Address = address;
			Arguments = arguments;
		}
	}

	public static class OscSender
	{
		public static event EventHandler<OscArgs> OnOscSend = delegate { };

		internal static void Send(OscAddress to, object[] arguments)
		{
			if(!OscTree.Deliver(to.Route, arguments))
			{
				OnOscSend(null, new OscArgs(OscTree.RouteToAddress(to.Route), arguments));
			}
		}

		internal static void Send(OscAddress to, object argument)
		{
			var arguments = new[] { argument };
			Send(to, arguments);
		}

		internal static void Send(OscAddress to)
		{
			Send(to, null);
		}


	}
}

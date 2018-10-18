using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms.Controls
{
	public class Led : yGui.Led, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public Led(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(int));

			Color = json.Color;
			Scale = json.Scale;
			Visible = json.Visible;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Blink", (args) =>
			{
				int time = 100;
				if(args.Length > 0)
				{
					if (args[0] is int) time = (int)args[0];
					else if (args[0] is float) time = (int)args[0];
				}
				Blink(time);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Color", (args) =>
			{
				Color = OscParser.ToColor(args);
			}));
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

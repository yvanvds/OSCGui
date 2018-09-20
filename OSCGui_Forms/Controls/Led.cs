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

			oscObject.Endpoints.Add(new OscTree.Endpoint("Blink", (args) =>
			{
				Blink(100);
			}));
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

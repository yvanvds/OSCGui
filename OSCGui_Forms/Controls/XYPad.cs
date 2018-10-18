using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace OSCGui_Forms.Controls
{
	class XYPad : yGui.XYPad, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public XYPad(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(Point));

			ForeGround = json.Color;
			Border = json.Color;
			Centered = json.Centered;
			ShowValue = json.ShowValue;
			OscObject.Targets = json.Targets;
			Visible = json.Visible;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Coordinate", (args) =>
			{
				Value = OscParser.ToPoint(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Centered", (args) =>
			{
				Centered = OscParser.ToBool(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ForegroundColor", (args) =>
			{
				ForeGround = OscParser.ToColor(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("BorderColor", (args) =>
			{
				Border = OscParser.ToColor(args);
			}));

			ValueChanged += XYPad_ValueChanged;
		}

		private void XYPad_ValueChanged(object sender, EventArgs e)
		{
			OscObject.Send(new object[] { Value.X, Value.Y });
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

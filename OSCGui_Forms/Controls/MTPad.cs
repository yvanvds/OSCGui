using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms.Controls
{
	public class MTPad : yGui.MTPad, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public MTPad(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(float));

			ForeGround = json.Color;
			BackGround = json.Background;
			OscObject.Targets = json.Targets;
			Visible = json.Visible;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ForegroundColor", (args) =>
			{
				ForeGround = OscParser.ToColor(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("BackgroundColor", (args) =>
			{
				BackGround = OscParser.ToColor(args);
			}));

			this.TouchChanged += MTPad_TouchChanged;
		}

		private void MTPad_TouchChanged(object sender, TouchArgs e)
		{
			OscObject.Send(new object[] {e.Id, e.Pos.X, e.Pos.Y});
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

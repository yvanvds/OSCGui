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
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Visible = OscParser.ToBool(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ForegroundColor", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { ForeGround = OscParser.ToColor(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("BackgroundColor", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { BackGround = OscParser.ToColor(args); });
			}));

			this.TouchChanged += MTPad_TouchChanged;
		}

		private void MTPad_TouchChanged(object sender, TouchArgs e)
		{
			int action = 0;
			if (e.Action == yGui.TouchAction.Pressed) action = 1;
			else if (e.Action == yGui.TouchAction.Released) action = -1;
			OscObject.Send(new object[] {e.Id, e.Pos.X, e.Pos.Y, e.Size, action });
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

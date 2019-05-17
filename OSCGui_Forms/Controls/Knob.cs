using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms.Controls
{
	public class Knob : yGui.Knob, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public Knob(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(float));

			Minimum = json.Minimum;
			Maximum = json.Maximum;
			Color = json.Color;
			DisplayName = json.Content as string;
			ShowValue = json.ShowValue;
			OscObject.Targets = json.Targets;
			Visible = json.Visible;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Value", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Value = OscParser.ToFloat(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Minimum", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Minimum = OscParser.ToFloat(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Maximum", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Maximum = OscParser.ToFloat(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ShowValue", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { ShowValue = OscParser.ToBool(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Visible = OscParser.ToBool(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Color", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Color = OscParser.ToColor(args); });
			}));

			this.ValueChanged += Knob_ValueChanged;
		}

		private void Knob_ValueChanged(object sender, EventArgs e)
		{
			OscObject.Send(Value);
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

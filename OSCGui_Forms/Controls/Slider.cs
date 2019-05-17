using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms.Controls
{
	public class Slider : yGui.Slider, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public Slider(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(float));

			Minimum = json.Minimum;
			Maximum = json.Maximum;
			ForeGround = json.Color;
			Handle = json.Handle;
			Background = json.Background;
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
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Background = OscParser.ToColor(args); });
			}));

			ValueChanged += Slider_ValueChanged;
		}

		private void Slider_ValueChanged(object sender, EventArgs e)
		{
			OscObject.Send(Value);
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

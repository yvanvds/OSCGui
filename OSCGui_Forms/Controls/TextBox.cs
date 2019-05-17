using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms.Controls
{
	class TextBox : Xamarin.Forms.Entry, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public TextBox(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(string));

			Text = json.Content as string;
			FontSize = json.FontSize;
			OscObject.Targets = json.Targets;
			IsVisible = json.Visible;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Text = OscParser.ToString(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("FontSize", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FontSize = OscParser.ToInt(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { IsVisible = OscParser.ToBool(args); });
			}));

			TextChanged += TextBox_TextChanged;
		}

		private void TextBox_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			OscObject.Send(Text);
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

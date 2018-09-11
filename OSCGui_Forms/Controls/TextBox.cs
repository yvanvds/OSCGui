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
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID));

			Text = json.Content as string;
			FontSize = json.FontSize;
			OscObject.Targets = json.Targets;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Text = OscParser.ToString(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("FontSize", (args) =>
			{
				FontSize = OscParser.ToInt(args);
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

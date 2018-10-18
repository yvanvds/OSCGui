using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms.Controls
{
	public class Label : Xamarin.Forms.Label, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public Label(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(int));

			Text = (string)json.Content;
			TextColor = json.Color;
			BackgroundColor = json.Background;
			FontSize = json.FontSize;

			if(json.Bold)
			{
				if (json.Italic) FontAttributes = Xamarin.Forms.FontAttributes.Bold | Xamarin.Forms.FontAttributes.Italic;
				else FontAttributes = Xamarin.Forms.FontAttributes.Bold;
			} else if (json.Italic)
			{
				FontAttributes = Xamarin.Forms.FontAttributes.Italic;
			}
			HorizontalTextAlignment = json.HAlign;
			IsVisible = json.Visible;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Text = OscParser.ToString(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("FontSize", (args) =>
			{
				FontSize = OscParser.ToInt(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				IsVisible = OscParser.ToBool(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("TextColor", (args) =>
			{
				TextColor = OscParser.ToColor(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("BackgroundColor", (args) =>
			{
				BackgroundColor = OscParser.ToColor(args);
			}));
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

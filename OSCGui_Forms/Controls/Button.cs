﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms.Controls
{
	public class Button : yGui.Button, OscTree.IOscObject
	{
		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

		public Button(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			oscObject = new OscTree.Object(new OscTree.Address(json.Name, json.UID), typeof(bool));

			Text = json.Content as string;
			ForeGround = json.Color;
			BackGround = json.Background;
			TextScale = json.TextScale;
			OscObject.Targets = json.Targets;
			IsToggle = json.IsToggle;
			Visible = json.Visible;

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Text = OscParser.ToString(args); });
				
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("TextScale", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { TextScale = (yGui.Scale)OscParser.ToInt(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Visible = OscParser.ToBool(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ForegroundColor", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { TextColor = OscParser.ToColor(args); });
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("BackgroundColor", (args) =>
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { BackgroundColor = OscParser.ToColor(args); });
			}));

			Pressed += Button_Pressed;
		}

		private void Button_Pressed(object sender, EventArgs e)
		{
			if(IsToggle)
			{
				OscObject.Send(Toggled);
			} else
			{
				OscObject.Send(null);
			}
		}

		public void Taint()
		{
			// not used on client
		}
	}
}

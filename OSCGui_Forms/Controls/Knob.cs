﻿using Newtonsoft.Json.Linq;
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
				Value = OscParser.ToFloat(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Minimum", (args) =>
			{
				Minimum = OscParser.ToFloat(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Maximum", (args) =>
			{
				Maximum = OscParser.ToFloat(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ShowValue", (args) =>
			{
				ShowValue = OscParser.ToBool(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
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

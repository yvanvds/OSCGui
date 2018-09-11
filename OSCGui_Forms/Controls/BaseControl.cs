using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace OSCGui_Forms.Controls
{
	public class BaseControl : ContentView
	{
		private OscGuiView parent;

		public BaseControl (OscGuiView parent)
		{
			this.parent = parent;

		}

		private int rows = 1;
		public int Rows
		{
			get => rows;
			set
			{
				rows = value;
				Grid.SetRowSpan(this, rows);
			}
		}

		private int columns = 1;
		public int Columns
		{
			get => columns;
			set
			{
				columns = value;
				Grid.SetColumnSpan(this, columns);
			}
		}

		private System.Drawing.Point pos = new System.Drawing.Point();
		public System.Drawing.Point Pos
		{
			get => pos;
			set
			{
				pos = value;
				Grid.SetColumn(this, pos.X);
				Grid.SetRow(this, pos.Y);
			}
		}

		public bool LoadJSON(JObject obj)
		{
			Rows = (int)obj["Rows"];
			Columns = (int)obj["Columns"];

			if (obj.ContainsKey("Child"))
			{
				JObject childObj = obj["Child"] as JObject;
				switch ((string)childObj["Type"])
				{
					case "Label":
						var label = new Label(childObj);
						Content = label;
						parent.OscTree.Add(label.OscObject);
						break;
					case "Button":
						var button = new Button(childObj);
						Content = button;
						parent.OscTree.Add(button.OscObject);
						break;
					case "Slider":
						var slider = new Slider(childObj);
						Content = slider;
						parent.OscTree.Add(slider.OscObject);
						break;
					case "Knob":
						var knob = new Knob(childObj);
						Content = knob;
						parent.OscTree.Add(knob.OscObject);
						break;
					case "TextBox":
						var textbox = new TextBox(childObj);
						Content = textbox;
						parent.OscTree.Add(textbox.OscObject);
						break;
					case "Led":
						var led = new Led(childObj);
						Content = led;
						parent.OscTree.Add(led.OscObject);
						break;
					case "XYPad":
						var xypad = new XYPad(childObj);
						Content = xypad;
						parent.OscTree.Add(xypad.OscObject);
						break;
					case "MTPad":
						var mtpad = new MTPad(childObj);
						Content = mtpad;
						parent.OscTree.Add(mtpad.OscObject);
						break;
				}				
			}

			return true;
		}
	}
}
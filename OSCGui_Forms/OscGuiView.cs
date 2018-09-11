using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace OSCGui_Forms
{
	public class OscGuiView : ContentView
	{
		private OscTree.Tree tree;
		public OscTree.Tree OscTree => tree;

		private int rows;
		private int columns;

		private string name = "Gui";
		public string Name
		{
			get => name;
			set
			{
				name = value;
				if (OscTree != null) OscTree.Address.Name = value;
			}
		}

		public OscGuiView()
		{
			BackgroundColor = new Color(0.1, 0.1, 0.1);
			Content = new Grid();
			tree = new OscTree.Tree(new OscTree.Address(Name));

		}

		public bool LoadJSON(string content)
		{
			JObject obj = JObject.Parse(content);
			rows = (int)obj["Rows"];
			columns = (int)obj["Columns"];
			Name = (string)obj["Name"];

			reset();

			OscTree.Address.Name = Name;
			OscTree.Address.ID = (string)obj["Uid"];

			JArray objects = obj["objects"] as JArray;
			foreach (var elm in objects)
			{
				int x = (int)elm["PosX"];
				int y = (int)elm["PosY"];

				var ctrl = new Controls.BaseControl(this);
				Add(ctrl, y, x);
				ctrl.LoadJSON(elm as JObject);
			}
			return true;
		}

		private void reset()
		{
			var grid = Content as Grid;
			grid.ColumnDefinitions.Clear();
			grid.RowDefinitions.Clear();
			grid.Children.Clear();
			OscTree.Children.List.Clear();

			for (int i = 0; i < rows; i++)
			{
				grid.RowDefinitions.Add(new RowDefinition());
			}

			for (int i = 0; i < columns; i++)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition());
			}
		}

		void Add(Controls.BaseControl obj, int row, int column)
		{
			obj.Rows = obj.Columns = 1;
			obj.Pos = new System.Drawing.Point(column, row);
			(Content as Grid).Children.Add(obj);
		}
	}
}
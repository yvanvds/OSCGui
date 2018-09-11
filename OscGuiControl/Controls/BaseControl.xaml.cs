using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OscGuiControl.Controls
{
	/// <summary>
	/// Interaction logic for BaseControl.xaml
	/// </summary>
	public partial class BaseControl : UserControl
	{
		ContextMenu editMenu;

		private OscGUI parent;

		private bool changed = false;

		public UIElement Child { get => Element.Child; }

		public BaseControl(OscGUI parent)
		{
			InitializeComponent();
			this.parent = parent;

			SetEditMode(false);
			editMenu = new ContextMenu();
			{
				var item = new MenuItem();
				item.Header = "Clear";
				item.Click += ClearItem;
				editMenu.Items.Add(item);
			}
			editMenu.Items.Add(new Separator());
			{
				var item = new MenuItem();
				item.Header = "Label";
				item.Click += AddLabel;
				editMenu.Items.Add(item);
			}
			{
				var item = new MenuItem();
				item.Header = "Button";
				item.Click += AddButton;
				editMenu.Items.Add(item);
			}
			{
				var item = new MenuItem();
				item.Header = "Slider";
				item.Click += AddSlider;
				editMenu.Items.Add(item);
			}
			{
				var item = new MenuItem();
				item.Header = "Knob";
				item.Click += AddKnob;
				editMenu.Items.Add(item);
			}
			{
				var item = new MenuItem();
				item.Header = "TextBox";
				item.Click += AddTextBox;
				editMenu.Items.Add(item);
			}
			{
				var item = new MenuItem();
				item.Header = "Led";
				item.Click += AddLed;
				editMenu.Items.Add(item);
			}
			{
				var item = new MenuItem();
				item.Header = "XY-Pad";
				item.Click += AddXYPad;
				editMenu.Items.Add(item);
			}
			{
				var item = new MenuItem();
				item.Header = "MultiTouch Pad";
				item.Click += AddMultiTouchPad;
				editMenu.Items.Add(item);
			}

			
		}

		public bool NeedsSaving()
		{
			if (changed) return true;
			if(Element.Child != null && (Element.Child as IJsonInterface).HasChanged())
			{
				return true;
			} 
			return false;
		}

		private void AddMultiTouchPad(object sender, RoutedEventArgs e)
		{
			var pad = new MTPad();
			pad.IsEnabled = false;
			pad.IsHitTestVisible = false;
			Element.Child = pad;
			parent.OscTree.Add(pad.OscObject);
			if (editMode) changed = true;
		}

		private void AddXYPad(object sender, RoutedEventArgs e)
		{
			var pad = new XYPad();
			pad.IsEnabled = false;
			pad.IsHitTestVisible = false;
			Element.Child = pad;
			parent.OscTree.Add(pad.OscObject);
			if (editMode) changed = true;
		}

		private void AddLed(object sender, RoutedEventArgs e)
		{
			var led = new Led();
			led.IsEnabled = false;
			led.IsHitTestVisible = false;
			Element.Child = led;
			parent.OscTree.Add(led.OscObject);
			if (editMode) changed = true;
		}

		private void AddKnob(object sender, RoutedEventArgs e)
		{
			var knob = new Knob();
			knob.IsEnabled = false;
			knob.IsHitTestVisible = false;
			Element.Child = knob;
			parent.OscTree.Add(knob.OscObject);
			if (editMode) changed = true;
		}

		private void AddTextBox(object sender, RoutedEventArgs e)
		{
			var textbox = new TextBox();
			textbox.IsEnabled = false;
			textbox.IsHitTestVisible = false;
			Element.Child = textbox;
			parent.OscTree.Add(textbox.OscObject);
			if (editMode) changed = true;
		}

		private void AddSlider(object sender, RoutedEventArgs e)
		{
			var slider = new Slider();

			slider.IsEnabled = false;
			slider.IsHitTestVisible = false;
			Element.Child = slider;
			parent.OscTree.Add(slider.OscObject);
			if (editMode) changed = true;
		}

		private void AddButton(object sender, RoutedEventArgs e)
		{
			var button = new Button();
			button.IsEnabled = false;
			button.IsHitTestVisible = false;
			Element.Child = button;
			parent.OscTree.Add(button.OscObject);
			if (editMode) changed = true;
		}

		private void AddLabel(object sender, RoutedEventArgs e)
		{
			var label = new Label();
			label.IsEnabled = false;
			label.IsHitTestVisible = false;
			Element.Child = label;
			parent.OscTree.Add(label.OscObject);
			if (editMode) changed = true;
		}

		private void ClearItem(object sender, RoutedEventArgs e)
		{
			if(Element.Child != null)
			{
				RemoveCurrentElement();
			}
			Element.Child = null;
			if (editMode) changed = true;
		}

		private int rows = 1;
		public int Rows {
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
		public System.Drawing.Point Pos {
			get => pos;
			set
			{
				pos = value;
				Grid.SetColumn(this, pos.X);
				Grid.SetRow(this, pos.Y);
			}
		}

		public bool Contains(System.Drawing.Point p)
		{
			return (p.X >= Pos.X
				&& p.X < Pos.X + Columns
				&& p.Y >= Pos.Y
				&& p.Y < Pos.Y + Rows) ;
		}
		
		protected bool editMode = false;
		public void SetEditMode(bool value)
		{
			editMode = value;
			Element.BorderThickness = value ? new Thickness(1) : new Thickness(0);
			Element.Background = value
				? new SolidColorBrush(Color.FromRgb(58, 58, 58))
				: new SolidColorBrush(Color.FromRgb(26, 26, 26));

			if(value)
			{
				ContextMenu = editMenu;
			} else
			{
				if(Element.Child is IContextMenu)
				{
					ContextMenu = (Element.Child as IContextMenu).Menu;
				} else
				{
					ContextMenu = null;
				}
			}

			if(!value)
			{
				Selected = false;
			}

			if(Child != null)
			{
				Child.IsEnabled = !value;
				Child.IsHitTestVisible = !value;
			}
		}

		private bool selected = false;
		public bool Selected
		{
			get => selected;
			set
			{
				selected = value;
				if(editMode)
				{
					Element.Background = selected
					? new SolidColorBrush(Color.FromRgb(70, 70, 100))
					: new SolidColorBrush(Color.FromRgb(70, 70, 70));
				}
				
			}
		}

		public new string Name
		{
			get
			{
				if(Element.Child != null)
				{
					return (Element.Child as FrameworkElement).Name;
				}
				return string.Empty;
			}
		}

		public JObject ToJSON()
		{
			JObject result = new JObject();
			result["PosX"] = Pos.X;
			result["PosY"] = Pos.Y;
			result["Rows"] = Rows;
			result["Columns"] = Columns;

			if(Element.Child != null)
			{
				result["Child"] = (Element.Child as IJsonInterface).ToJSON();
			}

			return result;
		}

		public bool LoadJSON(JObject obj)
		{
			SetEditMode(false);
			Rows = (int)obj["Rows"];
			Columns = (int)obj["Columns"];

			if(obj.ContainsKey("Child"))
			{
				JObject childObj = obj["Child"] as JObject;
				switch ((string)childObj["Type"])
				{
					case "Label":
						AddLabel(this, null);
						break;
					case "Button":
						AddButton(this, null);
						break;
					case "Slider":
						AddSlider(this, null);
						break;
					case "Knob":
						AddKnob(this, null);
						break;
					case "TextBox":
						AddTextBox(this, null);
						break;
					case "Led":
						AddLed(this, null);
						break;
					case "XYPad":
						AddXYPad(this, null);
						break;
					case "MTPad":
						AddMultiTouchPad(this, null);
						break;
				}
				if(Element.Child != null)
				{
					var child = (Element.Child as OscTree.IOscObject);
					parent.OscTree.Remove(child.OscObject);
					(Element.Child as IJsonInterface).LoadJSON(childObj);
					parent.OscTree.Add(child.OscObject);
				}
			}
			return true;
		} 

		public void RemoveCurrentElement()
		{
			if(Element.Child != null)
			{
				parent.OscTree.Remove((Element.Child as OscTree.IOscObject).OscObject);
			}
		}



		private void ElementBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			parent.OnLeftMouseDown(this);
		}

		private void ElementBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			parent.OnLeftMouseUp(this);
		}

		private void ElementBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			parent.OnMouseEnter(this);
		}
	}
}

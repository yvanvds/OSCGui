using Newtonsoft.Json.Linq;
using OscGuiControl;
using OscGuiControl.Controls;
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

namespace OscGuiControl
{
	/// <summary>
	/// Interaction logic for OscGui.xaml
	/// </summary>
	public partial class OscGUI : UserControl
	{
		private BaseControl[,] shadowGrid;

		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Tree tree;
		public OscTree.Tree OscTree => tree;

		static private int id = 1;

		static OscGUI()
		{
			properties = new PropertyCollection();
			properties.Add("Name");
		}

		public OscGUI()
		{
			InitializeComponent();

			Name = "Gui" + id++;
			tree = new OscTree.Tree(new OscTree.Address(Name));

			NameLabel.Content = Name;
			/*NameLabel.MouseDoubleClick += (s, e) =>
			{
				if (!EditMode) return;
				inspector?.Inspect(this as IOscObject);
			};*/

			Binding nameBinding = new Binding();
			nameBinding.Source = this;
			nameBinding.Path = new PropertyPath("Name");
			nameBinding.Mode = BindingMode.OneWay;
			nameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			BindingOperations.SetBinding(NameLabel, System.Windows.Controls.Label.ContentProperty, nameBinding);

			EditCheckbox.Checked += (s, e) =>
			{
				EditMode = true;
			};
			EditCheckbox.Unchecked += (s, e) =>
			{
				EditMode = false;
			};

			MergeButton.Click += (s, e) =>
			{
				MergeSelected();
			};

			SplitHButton.Click += (s, e) =>
			{
				splitHorizontal();
			};

			SplitVButton.Click += (s, e) =>
			{
				splitVertical();
			};

			UIGrid.MouseLeave += (s, e) =>
			{
				EndDrag(null);
			};

			UIGrid.MouseLeftButtonUp += (s, e) =>
			{
				EndDrag(null);
			};

			/*MouseDoubleClick += (s, e) =>
			{
				inspector?.Inspect(this as IOscObject);
			};*/
		}

		int rows;
		int columns;
		public void SetGridSize(int rows, int columns)
		{
			this.rows = rows;
			this.columns = columns;

			reset();

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < columns; c++)
				{
					Add(new BaseControl(this), r, c);
				}
			}
		}

		public new string Name
		{
			get => base.Name;
			set
			{
				base.Name = value;
				if (OscTree != null) OscTree.Address.Name = value;
			}
		}

		#region IO
		private bool guiChanged = false;
		public bool NeedsSaving()
		{
			if (guiChanged) return true;
			foreach (var elm in shadowGrid)
			{
				if (elm != null && elm.NeedsSaving())
				{
					return true;
				}
			}
			return false;
		}


		public string ToJSON()
		{
			JObject obj = new JObject();
			obj["Rows"] = rows;
			obj["Columns"] = columns;
			obj["Name"] = Name;
			obj["Uid"] = OscTree.Address.ID;

			JArray array = new JArray();
			foreach (var elm in shadowGrid)
			{
				if (elm != null)
				{
					array.Add(elm.ToJSON());
				}
			}
			obj["objects"] = array;

			guiChanged = false;

			return obj.ToString();
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

				var ctrl = new BaseControl(this);
				Add(ctrl, y, x);
				ctrl.LoadJSON(elm as JObject);
			}
			guiChanged = false;
			return true;
		}

		private void reset()
		{
			if (shadowGrid != null)
			{
				foreach (var elm in shadowGrid)
				{
					if (elm != null)
					{
						elm.RemoveCurrentElement();
					}
				}
			}
			shadowGrid = new BaseControl[columns, rows];

			UIGrid.ColumnDefinitions.Clear();
			UIGrid.RowDefinitions.Clear();
			UIGrid.Children.Clear();

			for (int i = 0; i < rows; i++)
			{
				UIGrid.RowDefinitions.Add(new RowDefinition());
			}

			for (int i = 0; i < columns; i++)
			{
				UIGrid.ColumnDefinitions.Add(new ColumnDefinition());
			}
		}
		#endregion IO

		#region Dragging
		private BaseControl draggingObj = null;

		private void startDrag(BaseControl obj)
		{
			draggingObj = obj;
			Mouse.OverrideCursor = Cursors.Hand;
		}

		public void EndDrag(BaseControl target)
		{
			Mouse.OverrideCursor = null;
			if (draggingObj == null)
			{
				return;
			}
			if (target == null)
			{
				draggingObj = null;
				return;
			}

			int targetRows = target.Rows;
			int targetColumns = target.Columns;
			target.Rows = draggingObj.Rows;
			target.Columns = draggingObj.Columns;
			draggingObj.Rows = targetRows;
			draggingObj.Columns = targetColumns;
			System.Drawing.Point targetPos = target.Pos;
			target.Pos = draggingObj.Pos;
			shadowGrid[target.Pos.X, target.Pos.Y] = target;
			draggingObj.Pos = targetPos;
			shadowGrid[draggingObj.Pos.X, draggingObj.Pos.Y] = draggingObj;

			draggingObj = null;
			guiChanged = true;
		}
		#endregion

		#region Merger
		public void MergeSelected()
		{
			// find low and high borders
			System.Drawing.Point min = new System.Drawing.Point(-1, -1);
			System.Drawing.Point max = new System.Drawing.Point(-1, -1);

			int selected = 0;
			foreach (var elm in shadowGrid)
			{
				if (elm != null && elm.Selected)
				{
					if (min.X > elm.Pos.X || min.X == -1) min.X = elm.Pos.X;
					if (min.Y > elm.Pos.Y || min.Y == -1) min.Y = elm.Pos.Y;
					if (max.X < elm.Pos.X + elm.Columns - 1 || max.X == -1) max.X = elm.Pos.X + elm.Columns - 1;
					if (max.Y < elm.Pos.Y + elm.Rows - 1 || max.Y == -1) max.Y = elm.Pos.Y + elm.Rows - 1;
					selected++;
				}
			}
			if (selected < 2) return; // don't merge a single object

			// set left upper element aside and remove the rest
			BaseControl keepElm = null;
			var p = new System.Drawing.Point(0, 0);
			for (; p.X < columns; p.X++)
			{
				p.Y = 0;
				for (; p.Y < rows; p.Y++)
				{
					if (p.Equals(min)) keepElm = shadowGrid[p.X, p.Y];
					else if (p.X >= min.X && p.X <= max.X && p.Y >= min.Y && p.Y <= max.Y)
					{
						if (shadowGrid[p.X, p.Y] != null)
						{
							UIGrid.Children.Remove(shadowGrid[p.X, p.Y]);
							shadowGrid[p.X, p.Y] = null;
						}
					}
				}
			}

			// now resize the kept element
			if (keepElm != null)
			{
				keepElm.Columns = max.X - min.X + 1;
				keepElm.Rows = max.Y - min.Y + 1;
				selectedObj = keepElm;
				SetSplitMode();
			}

			// see if any null elements have to be replaced
			p = new System.Drawing.Point(0, 0);
			for (; p.X < columns; p.X++)
			{
				for (; p.Y < rows; p.Y++)
				{
					if (!IsTaken(p))
					{
						var ctrl = new BaseControl(this);
						Add(ctrl, p.Y, p.X);
						ctrl.SetEditMode(true);
					}
				}
			}
			guiChanged = true;
		}

		#endregion Merger

		#region Splitter
		private void SetSplitMode()
		{
			if (selectedObj == null)
			{
				SplitHButton.IsEnabled = false;
				SplitVButton.IsEnabled = false;
			}
			else if (selectedObj.Rows > 1 && selectedObj.Columns > 1)
			{
				SplitHButton.IsEnabled = true;
				SplitVButton.IsEnabled = true;
			}
			else if (selectedObj.Rows > 1)
			{
				SplitHButton.IsEnabled = false;
				SplitVButton.IsEnabled = true;
			}
			else if (selectedObj.Columns > 1)
			{
				SplitHButton.IsEnabled = true;
				SplitVButton.IsEnabled = false;
			}
			else
			{
				SplitHButton.IsEnabled = false;
				SplitVButton.IsEnabled = false;
			}
		}

		private void splitHorizontal()
		{
			if (selectedObj == null) return;
			int rows = selectedObj.Rows;
			int columns = selectedObj.Columns;

			int newColumns = columns / 2;
			selectedObj.Columns = newColumns;

			var pos = selectedObj.Pos;
			pos.X += newColumns;

			var ctrl = new BaseControl(this);
			Add(ctrl, pos.Y, pos.X);
			ctrl.Columns = columns - newColumns;
			ctrl.Rows = rows;
			ctrl.SetEditMode(true);

			selectedObj.Selected = false;
			selectedObj = null;
			SetSplitMode();
			guiChanged = true;
		}

		private void splitVertical()
		{
			if (selectedObj == null) return;
			int rows = selectedObj.Rows;
			int columns = selectedObj.Columns;

			int newRows = rows / 2;
			selectedObj.Rows = newRows;

			var pos = selectedObj.Pos;
			pos.Y += newRows;

			var ctrl = new BaseControl(this);
			Add(ctrl, pos.Y, pos.X);
			ctrl.Columns = columns;
			ctrl.Rows = rows - newRows;
			ctrl.SetEditMode(true);

			selectedObj.Selected = false;
			selectedObj = null;
			SetSplitMode();
			guiChanged = true;
		}
		#endregion

		#region Selector
		private BaseControl selectedObj = null;
		private bool multiSelectInProgress = false;

		public void OnLeftMouseDown(BaseControl obj)
		{
			if (!EditMode) return;

			selectedObj = null;
			SetSplitMode();
			ClearAllSelections();

			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				multiSelectInProgress = true;
				EvaluateSelection(obj, true);
			}

			else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				startDrag(obj);
			}

			else
			{
				Inspect(obj.Child);
			}
		}

		public void OnMouseEnter(BaseControl obj)
		{
			if (!EditMode) return;
			if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
			{
				multiSelectInProgress = false;
			}
			if (!multiSelectInProgress) return;
			EvaluateSelection(obj, false);
		}

		public void OnLeftMouseUp(BaseControl obj)
		{
			if (!EditMode) return;
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				multiSelectInProgress = false;
			}
			else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				EndDrag(obj);
			}
			else
			{
				if (selectedObj == obj)
				{
					obj.Selected = false;
					selectedObj = null;
					SetSplitMode();
				}
				else
				{
					if (selectedObj != null)
					{
						selectedObj.Selected = false;
					}
					obj.Selected = true;
					selectedObj = obj;
					SetSplitMode();
				}
			}
		}

		private void ClearAllSelections()
		{
			foreach (var obj in shadowGrid)
			{
				if (obj != null) obj.Selected = false;
			}
		}

		private System.Drawing.Point selectionStart;
		private void EvaluateSelection(BaseControl obj, bool isStarting)
		{
			if (isStarting)
			{
				selectionStart = obj.Pos;
				obj.Selected = true;
			}
			else
			{
				System.Drawing.Point selectionEnd = obj.Pos;

				int minX = selectionStart.X < selectionEnd.X ? selectionStart.X : selectionEnd.X;
				int maxX = selectionStart.X < selectionEnd.X ? selectionEnd.X : selectionStart.X;

				int minY = selectionStart.Y < selectionEnd.Y ? selectionStart.Y : selectionEnd.Y;
				int maxY = selectionStart.Y < selectionEnd.Y ? selectionEnd.Y : selectionStart.Y;

				foreach (var elm in shadowGrid)
				{
					if (elm != null)
					{
						elm.Selected = (elm.Pos.X >= minX
						&& elm.Pos.X <= maxX
						&& elm.Pos.Y <= maxY
						&& elm.Pos.Y >= minY);
					}
				}
			}
		}
		#endregion Selector

		#region EditMode
		public bool EditMode
		{
			get => (bool)GetValue(EditModeProperty);
			set
			{
				//if ((bool)GetValue(EditModeProperty) == true && value == false)
				//{
				//	GenerateValueChangeEvent();
				//}
				SetValue(EditModeProperty, value);
				foreach (var elm in shadowGrid)
				{
					elm?.SetEditMode(value);
				}
			}
		}
		public static readonly DependencyProperty EditModeProperty =
			DependencyProperty.Register(nameof(EditMode), typeof(bool), typeof(OscGUI), new PropertyMetadata(false));
		#endregion EditMode

		/*
		#region ChangeEvent
		public static RoutedEvent ValueChangedEvent =
			EventManager.RegisterRoutedEvent(
				"Changed",
				RoutingStrategy.Bubble,
				typeof(RoutedEventHandler),
				typeof(OscGUI));

		public event RoutedEventHandler Changed
		{
			add { AddHandler(ValueChangedEvent, value); }
			remove { RemoveHandler(ValueChangedEvent, value); }
		}

		private void GenerateValueChangeEvent()
		{
			RoutedEventArgs args = new RoutedEventArgs(ValueChangedEvent, this);
			RaiseEvent(args);
		}
		#endregion ChangeEvent
	*/
		#region Inspector
		private OscInspectorGui inspector = null;
		public void SetInspector(OscInspectorGui inspector)
		{
			this.inspector = inspector;
		}

		private void Inspect(object obj)
		{
			inspector?.Inspect(obj as OscTree.IOscObject);
		}

		#endregion Inspector

		#region Utils
		/*T GetTemplateChild<T>(string name) where T : DependencyObject
		{
			var child = GetTemplateChild(name) as T;
			if (child == null)
				throw new NullReferenceException(name);
			return child;
		}*/


		void Add(BaseControl obj, int row, int column)
		{
			shadowGrid[column, row] = obj;
			obj.Rows = obj.Columns = 1;
			obj.Pos = new System.Drawing.Point(column, row);
			UIGrid.Children.Add(obj);
		}

		public bool IsTaken(System.Drawing.Point p)
		{
			foreach (var elm in shadowGrid)
			{
				if (elm != null)
				{
					if (elm.Contains(p)) return true;
				}
			}
			return false;
		}

		#endregion Utils
	}
}

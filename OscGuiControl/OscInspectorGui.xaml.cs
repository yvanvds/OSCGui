using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OscTree;

namespace OscGuiControl
{
	/// <summary>
	/// Interaction logic for OscInspectorGui.xaml
	/// </summary>
	public partial class OscInspectorGui : UserControl
	{
		private object currentObject = null;
		public object CurrentObject => currentObject;

		public OscInspectorGui()
		{
			InitializeComponent();

			PropertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
		}

		private void PropertyGrid_PropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
		{
			if(currentObject != null)
			{
				if(currentObject is OscTree.IOscObject)
				{
					(currentObject as OscTree.IOscObject).Taint();
				}
			}
		}

		public OscTree.Tree OscRoot { get; set; } = null;
		public OscGuiControl.IOscRoutePicker CustomRoutePicker = null;

		public void Inspect(object obj)
		{
			if (obj != null)
			{ 
				if(obj is IPropertyInterface)
				{
					PropertyGrid.PropertyDefinitions = (obj as IPropertyInterface).Properties.Collection;
				}
			}
			PropertyGrid.SelectedObject = obj;
			currentObject = obj;
		}

		private void ReceiversButton_Click(object sender, RoutedEventArgs e)
		{
			if (OscRoot == null) return;

			if(PropertyGrid.SelectedObject is OscTree.IOscObject)
			{
				var ctrl = PropertyGrid.SelectedObject as OscTree.IOscObject;
				ctrl.OscObject.Targets.UpdateScreenNames(OscRoot);

				if(CustomRoutePicker != null)
				{
					CustomRoutePicker.SetRoutes(ctrl.OscObject.Targets);
				} else
				{
					var dialog = new Windows.ReceiverList(OscRoot);
					dialog.List = ctrl.OscObject.Targets;
					dialog.ShowDialog();
					ctrl.OscObject.Targets = dialog.List;
					
				}
				ctrl.Taint();
			}
		}

        private void PropertyGrid_SelectedObjectChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            foreach(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem item in (sender as Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid).Properties)
            {
                item.Background = new SolidColorBrush(Colors.White);
                item.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}

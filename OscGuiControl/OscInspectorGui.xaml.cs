using System.Windows;
using System.Windows.Controls;
using OscTree;

namespace OscGuiControl
{
	/// <summary>
	/// Interaction logic for OscInspectorGui.xaml
	/// </summary>
	public partial class OscInspectorGui : UserControl
	{
		public OscInspectorGui()
		{
			InitializeComponent();
		}

		public OscTree.Tree OscRoot { get; set; } = null;

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
		}

		private void ReceiversButton_Click(object sender, RoutedEventArgs e)
		{
			if (OscRoot == null) return;

			if(PropertyGrid.SelectedObject is OscTree.IOscObject)
			{
				var ctrl = PropertyGrid.SelectedObject as OscTree.IOscObject;
				var dialog = new Windows.ReceiverList(OscRoot);
				ctrl.OscObject.Targets.UpdateScreenNames(OscRoot);
				dialog.List = ctrl.OscObject.Targets;
				dialog.ShowDialog();
				ctrl.OscObject.Targets = dialog.List;
			}
		}
	}
}

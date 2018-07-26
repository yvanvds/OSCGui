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
	/// Interaction logic for OscInspectorGui.xaml
	/// </summary>
	public partial class OscInspectorGui : UserControl
	{
		ISender currentObj;
		OscAddressList currentReceiver;

		public OscInspectorGui()
		{
			InitializeComponent();
		}

		public void Inspect(IOscObject ctrl)
		{
			if (ctrl != null)
			{ 
				PropertyGrid.PropertyDefinitions = ctrl.Properties.Collection;
				if(ctrl is ISender)
				{
					currentObj = ctrl as ISender;
					currentReceiver = currentObj.Receivers;
				}
				
			}
			PropertyGrid.SelectedObject = ctrl;
			
		}

		private void ReceiversButton_Click(object sender, RoutedEventArgs e)
		{
			if(PropertyGrid.SelectedObject is ISender)
			{
				ISender ctrl = PropertyGrid.SelectedObject as ISender;
				var dialog = new Windows.ReceiverList();
				dialog.List = ctrl.Receivers;
				dialog.ShowDialog();
				ctrl.Receivers = dialog.List;
			}
			
			
		}
	}
}

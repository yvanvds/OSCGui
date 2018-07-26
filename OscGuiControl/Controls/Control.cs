using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OscGuiControl.Controls
{
	public abstract class Control
	{
		public abstract UIElement UIObject { get; }

		public abstract bool IsEnabled { get; set; }
		public abstract bool IsHitTestVisible { get; set; }
	}
}

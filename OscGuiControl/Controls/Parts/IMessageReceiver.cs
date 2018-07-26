using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscGuiControl.Controls.Parts
{
	public interface IMessageReceiver
	{
		void SetMessage(string target, object[] arguments);
	}
}

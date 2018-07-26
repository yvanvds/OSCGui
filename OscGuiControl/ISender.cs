using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscGuiControl
{
	interface ISender
	{
		OscAddressList Receivers { get; set; }
	}
}

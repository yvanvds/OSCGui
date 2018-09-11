using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSCGui_Forms
{
	interface IJsonInterface
	{
		JObject ToJSON();
		bool LoadJSON(JObject obj);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace OscGuiControl
{
	public class PropertyCollection
	{
		private PropertyDefinitionCollection collection = new PropertyDefinitionCollection();
		public PropertyDefinitionCollection Collection { get => collection; }

		public void Add(string propName)
		{
			collection.Add(new PropertyDefinition()
			{
				TargetProperties = new string[] { propName }
			});
		}

		public void Add(string propName, string displayName)
		{
			collection.Add(new PropertyDefinition()
			{
				DisplayName = displayName,
				TargetProperties = new string[] { propName }
			});
		}

		public void Add(string propName, string displayName, string category)
		{
			collection.Add(new PropertyDefinition()
			{
				Category = category,
				DisplayName = displayName,
				TargetProperties = new string[] { propName }
			});
		}
	}
}

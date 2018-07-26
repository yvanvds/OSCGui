using Newtonsoft.Json.Linq;

namespace OscGuiControl
{
	public interface IOscObject
	{
		PropertyCollection Properties { get; }
		OscObjectRoutes Routes { get; }
		OscAddress Address { get; }

		string Name { get; }
		string UID { get; }

		JObject ToJSON();
		bool LoadJSON(JObject obj);
	}
}

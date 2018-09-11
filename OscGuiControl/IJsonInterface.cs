using Newtonsoft.Json.Linq;

namespace OscGuiControl
{
	public interface IJsonInterface
	{
		JObject ToJSON();
		bool LoadJSON(JObject obj);

		bool HasChanged();
	}
}

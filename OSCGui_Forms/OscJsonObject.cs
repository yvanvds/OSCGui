using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;

namespace OSCGui_Forms
{
	internal class OscJsonObject
	{
		private JObject obj = new JObject();

		public OscJsonObject(string type, string uid, string name)
		{
			obj["Type"] = type;
			obj["UID"] = uid;
			obj["Name"] = name;
		}

		public OscJsonObject(JObject obj)
		{
			this.obj = obj;
		}

		public JObject Get()
		{
			return obj;
		}

		public string Type => (string)obj["Type"];
		public string UID => (string)obj["UID"];
		public string Name => (string)obj["Name"];

		public object Content
		{
			set => obj["Content"] = value as string;
			get
			{
				if (obj.ContainsKey("Content")) return (string)obj["Content"];
				return string.Empty;
			}
		}

		public OscTree.Routes Targets
		{
			set
			{
				if (value != null && value.Count > 0)
				{
					JObject targets = new JObject();
					foreach (var target in value)
					{
						JObject obj = new JObject();
						if (target.Replacements != null)
						{
							obj["Replacements"] = new JObject();
							foreach (var replacement in target.Replacements)
							{
								obj["Replacements"]["" + replacement.Key] = replacement.Value;
							}
						}
						if (target.ValueOverrideMethodName != string.Empty)
						{
							obj["Method"] = target.ValueOverrideMethodName;
						}
						targets[target.OriginalName] = obj;
					}
					obj["Targets"] = targets;
				}
			}
			get
			{
				if (obj.ContainsKey("Targets"))
				{
					var targets = obj["Targets"] as JObject;
					var result = new OscTree.Routes();
					foreach (var target in targets)
					{
						OscTree.Route route = new OscTree.Route((string)target.Key, OscTree.Route.RouteType.ID);
						var content = target.Value as JObject;
						if (content.ContainsKey("Replacements"))
						{
							var replacements = content["Replacements"] as JObject;
							if (replacements.Count > 0)
							{
								route.Replacements = new Dictionary<int, string>();
								foreach (var replacement in replacements)
								{
									route.Replacements[Convert.ToInt32(replacement.Key)] = (string)replacement.Value;
								}
							}
						}
						if (content.ContainsKey("Method"))
						{
							route.ValueOverrideMethodName = (string)content["Method"];
						}
						else
						{
							route.ValueOverrideMethodName = string.Empty;
						}

						result.Add(route);
					}
					return result;
				}
				return new OscTree.Routes();
			}
		}

		public Color Color
		{
			set => obj["Color"] = value.ToString();
			get
			{
				if (obj.ContainsKey("Color"))
				{
					return Color.FromHex((string)obj["Color"]);
				}
				return yGui.Colors.Default;
			}
		}

		public Color Background
		{
			set => obj["Background"] = value.ToString();
			get
			{
				if (obj.ContainsKey("Background"))
				{
					return Color.FromHex((string)obj["Background"]);
				}
				return yGui.Colors.ElementBackground;
			}
		}

		public Color Handle
		{
			set => obj["Handle"] = value.ToString();
			get
			{
				if (obj.ContainsKey("Handle"))
				{
					return Color.FromHex((string)obj["Handle"]);
				}
				return yGui.Colors.White;
			}
		}


		public double FontSize
		{
			set => obj["FontSize"] = value;
			get
			{
				if (obj.ContainsKey("FontSize"))
				{
					return (double)obj["FontSize"];
				}
				return 12.0;
			}
		}

		public yGui.Scale TextScale
		{
			set => obj["TextScale"] = value.ToString();
			get
			{
				if (obj.ContainsKey("TextScale"))
				{
					switch ((string)obj["TextScale"])
					{
						case "Smallest":
							return yGui.Scale.Smallest;
						case "Small":
							return yGui.Scale.Small;
						case "Normal":
							return yGui.Scale.Normal;
						case "Big":
							return yGui.Scale.Big;
						case "Huge":
							return yGui.Scale.Huge;
					}
				}
				return yGui.Scale.Normal;
			}
		}

		public bool Bold
		{
			get
			{
				if (obj.ContainsKey("FontWeight"))
				{
					return ((int)obj["FontWeight"]) > 300;
				}
				return false;
			}
		}

		public bool Italic
		{
			get
			{
				if (obj.ContainsKey("FontStyle"))
				{
					switch ((string)obj["FontStyle"])
					{
						case "Italic":
							return true;
						case "Oblique":
							return true;
					}
				}
				return false;
			}
		}

		public Xamarin.Forms.TextAlignment HAlign
		{
			get
			{
				if (obj.ContainsKey("HAlign"))
				{
					switch ((string)obj["HAlign"])
					{
						case "Center":
							return TextAlignment.Center;
						case "Left":
							return TextAlignment.Start;
						case "Right":
							return TextAlignment.End;
						case "Stretch":
							return TextAlignment.Start;
					}
				}
				return TextAlignment.Start;
			}
		}

		public float Minimum
		{
			set => obj["Minimum"] = value;
			get
			{
				if (obj.ContainsKey("Minimum")) return (float)obj["Minimum"];
				return 0.0f;
			}
		}

		public float Maximum
		{
			set => obj["Maximum"] = value;
			get
			{
				if (obj.ContainsKey("Maximum")) return (float)obj["Maximum"];
				return 0.0f;
			}
		}

		public bool ShowValue
		{
			set => obj["ShowValue"] = value;
			get
			{
				if (obj.ContainsKey("ShowValue")) return (bool)obj["ShowValue"];
				return false;
			}
		}

		public bool Visible
		{
			set => obj["Visible"] = value;
			get
			{
				if (obj.ContainsKey("Visible")) return (bool)obj["Visible"];
				return true;
			}
		}

		public bool IsToggle
		{
			set => obj["IsToggle"] = value;
			get
			{
				if (obj.ContainsKey("IsToggle")) return (bool)obj["IsToggle"];
				return false;
			}
		}

		public float Scale
		{
			set => obj["Scale"] = value;
			get
			{
				if (obj.ContainsKey("Scale")) return (float)obj["Scale"];
				return 1f;
			}
		}

		public bool Centered
		{
			set => obj["Centered"] = value;
			get
			{
				if (obj.ContainsKey("Centered")) return (bool)obj["Centered"];
				return true;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OscGuiControl
{
	

	internal static class OscParser
	{
		internal static string ToString(object[] arguments)
		{
			if (arguments == null) return string.Empty;
			if (arguments.Length < 1) return string.Empty;
			return Convert.ToString(arguments[0]);
		}

		internal static float ToFloat(object[] arguments)
		{
			if (arguments == null) return 0;
			if (arguments.Length < 1) return 0;
			return (float)Convert.ToDouble(arguments[0]);
		}

		internal static int ToInt(object[] arguments)
		{
			if (arguments == null) return 0;
			if (arguments.Length < 1) return 0;
			return (int)Convert.ToInt32(arguments[0]);
		}

		internal static bool ToBool(object[] arguments)
		{
			if (arguments == null) return false;
			if (arguments.Length < 1) return false;
			return (bool)Convert.ToBoolean(arguments[0]);
		}

		internal static Point ToPoint(object[] arguments)
		{
			if (arguments == null) return new Point(0,0);
			if (arguments.Length < 1) return new Point(0, 0);
			if(arguments.Length < 2)
			{
				double value = Convert.ToDouble(arguments[0]);
				return new Point(value, value);
			}
			else
			{
				return new Point(Convert.ToDouble(arguments[0]), Convert.ToDouble(arguments[1]));
			}
		}

		internal static Color ToColor(object[] arguments)
		{
			if (arguments == null) return yGuiWPF.Colors.Default;
			if (arguments.Length < 1) return yGuiWPF.Colors.Default;
			if (arguments.Length < 3)
			{
				byte value = Convert.ToByte(arguments[0]);
				return Color.FromRgb(value, value, value);
			}
			byte r = Convert.ToByte(arguments[0]);
			byte g = Convert.ToByte(arguments[1]);
			byte b = Convert.ToByte(arguments[2]);
			return Color.FromRgb(r, g, b);
		}
	}
}

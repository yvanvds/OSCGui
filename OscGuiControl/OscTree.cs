using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shortid;

namespace OscGuiControl
{

	public class OscTree
	{
		public static OscTree Root = new OscTree();

		public static string GenerateUID()
		{
			return ShortId.Generate(true);
		}

		public static string RouteToAddress(List<string> route)
		{
			if (route.Count == 0) return string.Empty;
			if (!route[0].Equals(Root.Address.UID)) return string.Empty;
			return "/" + Root.Name + Root.routeToAddress(route, 1);
		}

		public static List<string> AddressToRoute(string address)
		{
			List<string> result = address.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToList();
			return Root.addressToRoute(result, 0).ToList();
		}

		public static bool Deliver(List<string> route, object[] arguments)
		{
			if (!route[0].Equals("OscRoot")) return false;
			var obj = Root.Find(route, 1);
			if (obj != null)
			{
				return obj.Routes.Deliver(route.Last(), arguments);
			}
			return false;
		}

		public static bool GetLevel(OscTree tree, out int level)
		{
			level = 0;
			return Root.findTreeLevel(tree, ref level);
		}

		public static bool GetLevel(IOscObject obj, out int level)
		{
			level = 0;
			return Root.findObjectLevel(obj, ref level);
		}

		private Dictionary<string, OscTree> trees;
		public Dictionary<string, OscTree> Trees => trees;

		private Dictionary<string, IOscObject> objects;
		public Dictionary<string, IOscObject> Objects => objects;

		private OscAddress Address;

		static OscTree()
		{
			Root.Address.UID = Root.Address.Name = "OscRoot";
			Root.Address.CreateRoute(new List<string>());
		}

		public OscTree() {
			trees = new Dictionary<string, OscTree>();
			objects = new Dictionary<string, IOscObject>();
			Address = new OscAddress();
		}

		public string Name {
			get => Address.Name;
			set => Address.Name = value;
		}

		public string UID
		{
			get => Address.UID;
			set => Address.UID = value;
		}

		public void Add(OscTree tree)
		{
			Trees.Add(tree.Address.UID, tree);
			tree.Address.CreateRoute(Address.Route);
		}

		public void Remove(string uid)
		{
			if(Trees.ContainsKey(uid))
			{
				Trees.Remove(uid);
			} else if (objects.ContainsKey(uid))
			{
				objects.Remove(uid);
			}
		}

		public void Add(IOscObject obj)
		{
			objects.Add(obj.Address.UID, obj);
			obj.Address.CreateRoute(Address.Route);
		}

		private IOscObject Find(List<string> route, int pos)
		{
			if (route.Count <= pos) return null;

			foreach(var tree in Trees)
			{
				if(tree.Key.Equals(route[pos]))
				{
					return tree.Value.Find(route, pos + 1);
				}
			}

			foreach(var obj in objects)
			{
				if(obj.Key.Equals(route[pos]))
				{
					return obj.Value;
				}
			}

			return null;
		}

		private string routeToAddress(List<string> route, int pos)
		{
			if (route.Count <= pos) return string.Empty;

			foreach (var tree in Trees)
			{
				if(tree.Key.Equals(route[pos]))
				{
					return "/" + tree.Value.Name + tree.Value.routeToAddress(route, pos+1);
				}
			}

			foreach(var obj in objects)
			{
				if(obj.Key.Equals(route[pos]))
				{
					string result = "/" + obj.Value.Name;
					if (route.Count > pos)
					{
						string routeEnd = route[pos + 1];
						if (obj.Value.Routes.List.Contains(routeEnd))
						{
							result += "/" + routeEnd;
						}
					}
					return result;
				}
			}

			return string.Empty;
		}

		private LinkedList<string> addressToRoute(List<string> address, int pos)
		{
			if (address.Count <= pos) return new LinkedList<string>();

			foreach(var tree in Trees)
			{
				if(tree.Value.Name.Equals(address[pos]))
				{
					var route = tree.Value.addressToRoute(address, pos + 1);
					route.AddFirst(tree.Key);
					return route;
				}
			}

			foreach(var obj in objects)
			{
				if(obj.Value.Name.Equals(address[pos]))
				{
					var route = obj.Value.Routes.AddressToRoute(address, pos + 1);
					route.AddFirst(obj.Key);
					return route;
				}
			}

			return new LinkedList<string>();
		} 

		private bool findTreeLevel(OscTree tree, ref int currentLevel)
		{
			currentLevel++;

			foreach(var elm in trees.Values)
			{
				if (elm.Equals(tree)) return true;
			}

			foreach(var elm in trees.Values)
			{
				if (elm.findTreeLevel(tree, ref currentLevel)) return true;
			}

			return false;
		}

		private bool findObjectLevel(IOscObject obj, ref int currentLevel)
		{
			currentLevel++;

			foreach(var elm in objects.Values)
			{
				if (elm.Equals(obj)) return true;
			}

			foreach(var elm in trees.Values)
			{
				if (elm.findObjectLevel(obj, ref currentLevel)) return true;
			}

			return false;
		}
	}
}

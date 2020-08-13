using System;
using System.Collections.Generic;
using System.Text;

namespace FirestoneJsonAchievementsMaker
{
	struct Layer
	{
		public Layer(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}
		public string Value { get; }
		public string Name { get; }

		public readonly static IList<Layer> Divisions = new List<Layer>(5)
		{
			new Layer("Bronze", "5"),
			new Layer("Silver", "4"),
			new Layer("Gold", "3"),
			new Layer("PLatinum", "2"),
			new Layer("Diamond", "1")
		};

		public static IList<Layer> MakeLayers(string[] list)
		{
			List<Layer> result = new List<Layer>(list.Length);
			for (int i = 0; i < list.Length; i++)
			{
				result.Add(new Layer(list[i++], list[i]));
			}
			return result;
		}

	}
}

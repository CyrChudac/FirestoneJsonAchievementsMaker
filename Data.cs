using System;
using System.Collections.Generic;
using System.Text;

namespace FirestoneJsonAchievementsMaker
{
    class Data
    {
		public Data( string id, string name, string description, string art, IList<Layer> layers,
			string beforeLayerName, string afterLayerName, bool ranked, Format format,
			string requirements, string resetEvents, bool ok)
		{
			this.Id = id;
			this.Name = name;
			this.Description = description;
			this.Art = art;
			this.Layers = layers;
			this.BeforeLayerName = beforeLayerName;
			this.AfterLayerName = afterLayerName;
			this.Ranked = ranked;
			this.Format = format;
			this.Requirements = requirements;
			this.ResetEvents = resetEvents;
			this.Ok = ok;
		}
		public string Id { get; }
		public string Name { get; }
		public string Description { get; }
		public string Art { get; }
		public IList<Layer> Layers { get; }
		public string BeforeLayerName { get; }
		public string AfterLayerName { get; }
		public bool Ranked { get; }
		public Format Format { get; }
		public string Requirements { get; }
		public string ResetEvents { get; }
		public bool Ok { get; }
	}
}

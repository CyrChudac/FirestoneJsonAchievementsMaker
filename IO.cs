using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FirestoneJsonAchievementsMaker
{
    class IO : IDisposable
    {
		readonly StreamReader readFrom;
		readonly StreamWriter writeTo;
		public IO(string readFrom, string writeTo)
		{
			this.readFrom = new StreamReader(new FileStream(readFrom, FileMode.Open));
			this.writeTo = new StreamWriter(new FileStream(writeTo, FileMode.Append));
		}
		public bool CanRead  => !readFrom.EndOfStream;
        public Data Read()
        {
			StringBuilder sb = new StringBuilder();
			int next;
			while( (next = readFrom.Read()) != -1 && (char)next != 'đ' )
			{
				sb.Append((char)next); 
			}
			if (string.IsNullOrWhiteSpace(sb.ToString()))
			{
				return null;
			}
			StringReader sr = new StringReader(sb.ToString());
			string id = null;
			string name = null;
			string description = null;
			string art = null;
			IList<Layer> layers = null;
			string beforeLayerName = null;
			string afterLayerName = null;
			bool ranked = true;
			Format format = Format.Invalid;
			string requirements = null;
			string resetEvents = null;

			string line;
			while((line = sr.ReadLine()) != null)
			{
				if(string.IsNullOrWhiteSpace(line))
				{
					continue;
				}
				string value = line.Substring(line.IndexOf(' ') + 1);
				string valName = line.Substring(0, line.IndexOf(' '));
				switch (valName)
				{
					case "id":
						id = value;
						break;
					case "name":
						name = value;
						break;
					case "text":
						description = value;
						break;
					case "artName":
						art = value;
						break;
					case "layers":
						if (value == "divisions")
						{
							layers = Layer.Divisions;
							beforeLayerName = @"{ ""type"": ""RANKED_MIN_LEAGUE"", ""values"": [""";
							afterLayerName = @"""] }";
						}
						else
						{
							layers = Layer.MakeLayers(value.Split(' '));
						}
						break;
					case "layerReqStart":
						beforeLayerName = value;
						break;
					case "layerReqEnd":
						afterLayerName = value;
						break;
					case "ranked":
						if (value == "true")
							ranked = true;
						else 
							ranked = false;
						break;
					case "format":
						if (value == "wild")
							format = Format.Wild;
						else if (value == "standard")
							format = Format.Standard;
						else if (value == "any")
							format = Format.Any;
						else format = Format.Invalid;
						break;
					case "requirements":
						requirements = string.Empty;
						while ((line = sr.ReadLine()) != null && line != "]")
						{
							requirements += line;
						}
						break;
					case "resetEvents":
						resetEvents = value;
						break;

				}
			}

			bool ok = !(id == null || name == null || description == null || art == null || layers == null ||
				beforeLayerName == null || afterLayerName == null || format == Format.Invalid ||
				requirements == null || resetEvents == null);

			return new Data(id, name, description, art, layers, beforeLayerName, afterLayerName,
				ranked, format, requirements, resetEvents, ok);
		}
		public void StartWriting()
		{
			writeTo.Write("{\n");
		}
		public void EndWriting()
		{
			writeTo.Write("\n}");
		}
		public void Write(Data d, bool isFirst = false)
		{
			DownloadCardsFile();
			CardData(d.Art, out string cardId, out string cardType);
			string text = string.Empty;
			for (int i = 0; i < d.Layers.Count; i++)
			{
				Layer layer = d.Layers[i];
				string formatText;
				if(d.Format == Format.Any)
				{
					formatText = @"""0"",""1"",""2""";
				}
				else
				{
					formatText = @"""" + (int)d.Format + @"""";
				}
				text += (isFirst && i == 0 ? "" : ",\n") +
					@"	{
		""id"": """ + d.Id + "_" + i + @""",
		""type"": """ + d.Id + @""",
		""name"": """ + d.Name + @""",
		""icon"": ""boss_victory"", //n
		""root"": " + (i == 0).ToString().ToLower() + @",
		""canBeCompletedOnlyOnce"": false, //n
		""priority"": " + (i - d.Layers.Count) + @",
		""displayName"": ""Achievement completed: " + d.Name + @" (" + layer.Name + @")"",
		""emptyText"": """ + d.Description + @""",
		""completedText"": ""Completed for " + layer.Name + @" or better"",
		""displayCardId"": """ + cardId + @""",
		""displayCardType"": """ + cardType.ToLower() + @""",
		""difficulty"": ""rare"", //n
		""maxNumberOfRecords"": 3, //n
		""points"": 17.0, //n
		""requirements"": [
			{ ""type"": ""GAME_TYPE"", ""values"": [""7""" + (!d.Ranked ? @",""8""" : "") + @"] },
			" + d.BeforeLayerName  + layer.Value + d.AfterLayerName + @", //" + layer.Name.ToLower() + @"
			{ ""type"": ""RANKED_FORMAT_TYPE"", ""values"": [" + formatText + @"] },
		" + d.Requirements + @"
		],
		""resetEvents"": " + d.ResetEvents + @"
	}";
			}
			writeTo.Write(text);
			writeTo.Flush();
		}

		string cardsSite = @"https://raw.githubusercontent.com/Zero-to-Heroes/hs-reference-data/master/projects/reference-data/src/lib/cards.json";
		string webData;
		void DownloadCardsFile()
		{
			System.Net.WebClient wc = new System.Net.WebClient();
			byte[] raw = wc.DownloadData(cardsSite);

			webData = System.Text.Encoding.UTF8.GetString(raw);
		}
		void CardData(string cardName, out string id, out string type)
		{
			int index = webData.IndexOf(@"""name"": """ + cardName + @"""");

			FindEnclosingsForIndex(index, out int first, out int second);
			int idIndex = webData.IndexOf(@"""id""", first, second - first);
			id = webData.Substring(idIndex + 7, webData.IndexOf("\"", idIndex + 7) - idIndex - 7);
			int typeIndex = webData.IndexOf(@"""type""", first, second - first);
			type = webData.Substring(typeIndex + 9, webData.IndexOf("\"", typeIndex + 9) - typeIndex - 9);
		}

		public void Dispose()
		{
			writeTo.Dispose();
			readFrom.Dispose();
		}

		void FindEnclosingsForIndex(int index, out int open, out int close)
		{
			int incr = index;
			int brack = 0;
			while (brack >= 0)
			{
				if (webData[incr] == '{')
				{
					brack++;
				}
				else if (webData[incr] == '}')
				{
					brack--;
				}
				incr++;
			}
			close = incr;

			brack = 0;
			int decr = index;
			while (brack <= 0)
			{
				if (webData[decr] == '{')
				{
					brack++;
				}
				else if (webData[decr] == '}')
				{
					brack--;
				}
				decr--;
			}
			open = decr;
		}
	}
}

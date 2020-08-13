using System;
using System.Collections.Generic;

namespace FirestoneJsonAchievementsMaker
{
    class Program
    {
		static void Main(string[] args)
		{
			if (args.Length <= 1 || args[0] == "-h")
			{
				PrintHelp();
			}
			else
			{
				IO io = new IO(args[0], args[1]);
				io.StartWriting();
				bool first = true;
				while (io.CanRead)
				{
					Data d = io.Read();
					if(d == null)
					{
						break;
					}
					if (!d.Ok)
					{
						if (d.Id != null)
							System.Console.WriteLine("Your input file does not have correct format on " + d.Id);
						else
							System.Console.WriteLine("Your input file does not have correct format");
					}
					else
					{
						io.Write(d, first);
						first = false;
					}
				}
				io.EndWriting();
				io.Dispose();
			}
		}

		static void PrintHelp()
		{
			Console.WriteLine(
			@"this program takes 2 arguments:
	1) path to file with defined achievemnts
	2) path to file where to write the result
the defined achievements need to have following structure:

id wild_amazing_plays_fatigue_5x
name The end is comming!
text Win a Ranked game where you take fatigue damage at least 5 times
artName Elixir of Vim
layers Bronze 5 Silver 4 Gold 3 Platinum 2 Diamond 1
layerReqStart { ""type"": ""RANKED_MIN_LEAGUE"", ""values"": [""
layerReqEnd ""] }
ranked true
format wild
requirements [
	{ ""type"": ""FATIGUE_DAMAGE"", ""values"": [""15"", ""AT_LEAST""]
	}
]
resetEvents [""GAME_START""]
đ

you can have any number of these definitions in the file

you can also just write 'divisions' into the layers slot, and than omitt the layerReqStart and end

to run this program, you need to be conected to the internet");
		}
    }
}


using System.IO;
using static System.Environment;
using System;
using System.Linq;

namespace NextStopAnnouncementGenerator.App
{
	internal class Program
	{
		private static readonly string NextStopPath = Path.Combine(GetFolderPath(SpecialFolder.Desktop), "next_stop_announcements");
		private static readonly Synth Synth = new Synth();

		internal static void Main()
		{
			while (true)
			{
				Console.WriteLine("\n\nCOMMANDS:");
				Console.WriteLine("\trun - Process all lines in Desktop\\next_stop_announcements\\next_stops.txt");
				Console.WriteLine("\tlist voice - List all installed voices");
				Console.WriteLine("\tshow voice - Show info about the current voice");
				Console.WriteLine("\tshow voice NUMBER - Show info about voice NUMBER");
				Console.WriteLine("\tset voice NUMBER - Set the voice to voice NUMBER");
				Console.WriteLine("\texit - Close the application.");
				Console.Write("command: ");
				var command = Console.ReadLine();
				Console.WriteLine();
				ParseInput(command);
			}
		}

		private static void ParseInput(string input)
		{
			input = input.Trim();
			var compare = StringComparison.CurrentCultureIgnoreCase;
			if ("exit".Equals(input, compare))
			{
				Exit(0);
			}
			else if ("run".Equals(input, compare))
			{
				RunSynth();
				return;
			}
			else if ("list voice".Equals(input, compare))
			{
				Synth.ListVoices();
				return;
			}
			else if (input.StartsWith("show voice", compare))
			{
				if ("show voice".Equals(input, compare))
				{
					Console.WriteLine(Synth.CurrentVoice.ToString());
					return;
				}
				else
				{
					if (int.TryParse(input.Split().Last(), out var num))
					{
						Console.WriteLine(Synth.Voices[num - 1].ToString());
						return;
					}
				}
			}
			else if (input.StartsWith("set voice", compare))
			{
				if (int.TryParse(input.Split().Last(), out var num))
				{
					Synth.ChangeVoice(num - 1);
					return;
				}
			}
			Console.WriteLine("bad input, try again.");
		}

		private static void RunSynth()
		{
			var path = Path.Combine(NextStopPath, "next_stop.txt");
			var lines = File.ReadAllLines(path);
			foreach (var line in lines)
			{
				Synth.CreateWavFile(NextStopPath, line, "Now approaching,", Console.WriteLine);
			}
			Exit(0);
		}

	}
}

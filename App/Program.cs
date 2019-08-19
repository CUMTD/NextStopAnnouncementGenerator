
using System.IO;
using static System.Environment;
using System;
using System.Linq;
using Console = Colorful.Console;
using System.Drawing;

namespace NextStopAnnouncementGenerator.App
{
	internal class Program
	{
		private static readonly Synth Synth = new Synth();

		internal static void Main()
		{
			Console.WriteAscii("Next Stop Speaker");
			while (true)
			{		
				var command = GetCommand();
				Console.WriteLine();
				ParseInput(command);
			}
		}

private static string GetCommand()
		{
			Console.WriteLine("\n\nCOMMANDS:", Color.White);
			ListCommand("run", "Process all lines in Desktop\\next_stop_announcements\\next_stops.txt");
			ListCommand("list voice", "List all installed voices");
			ListCommand("show voice", "Show info about the current voice");
			ListCommand("show voice", "Show info about the indicated voice", "NUMBER");
			ListCommand("set voice", "Set the voice based on the number in show voice", "NUMBER");
			ListCommand("exit", "Close the application");
			Console.Write(">  ", Color.White);
			return Console.ReadLine();
		}

		private static void ListCommand(string name, string description, string arg = null)
		{
			Console.Write($"\t{name}");
			if (!string.IsNullOrEmpty(arg))
			{
				Console.Write($" {arg}", Color.Green);
			}
			Console.WriteLine($" - {description}", Color.NavajoWhite);
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
				Synth.Run(Path.Combine(GetFolderPath(SpecialFolder.Desktop), "next_stop_announcements"));
				Exit(0);
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
					Synth.CurrentVoice.Print();
					return;
				}
				else
				{
					if (int.TryParse(input.Split().Last(), out var num))
					{
						Synth.Voices[num - 1].Print();
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
			Console.WriteLine("bad input, try again.", Color.Yellow);
		}

	}
}

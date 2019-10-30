using System;
using System.IO;
using static System.Environment;
using NextStopAnnouncementGenerator.Google;
using System.Threading.Tasks;

namespace NextStopAnnouncementGenerator.App
{
	internal class Program
	{
		internal static async Task Main()
		{
			var googleTextToSpeechClient = new GoogleTextToSpeechSynthesizer(
				Path.Combine(GetFolderPath(SpecialFolder.Desktop), "next_stop_announcements"),
				"next_stop.csv",
				Console.WriteLine
			);
			await googleTextToSpeechClient.Run();

			Console.WriteLine("\nDone!");
			Console.ReadLine();
		}

	}
}

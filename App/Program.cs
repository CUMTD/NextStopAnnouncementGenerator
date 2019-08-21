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
			var googleTextToSpeechClient = new GoogleTextToSpeechClient(Path.Combine(GetFolderPath(SpecialFolder.Desktop), "next_stop_announcements"));
			await googleTextToSpeechClient.Run(Console.WriteLine);

			Console.WriteLine("Done!");
			Console.ReadLine();
		}

	}
}

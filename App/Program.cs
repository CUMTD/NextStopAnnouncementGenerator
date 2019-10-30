using System;
using System.IO;
using static System.Environment;
using NextStopAnnouncementGenerator.Google;
using System.Threading.Tasks;
using NextStopAnnouncementGenerator.App.Config;

namespace NextStopAnnouncementGenerator.App
{
	internal class Program
	{
		internal static async Task Main()
		{
			var config = ConfigReader.ReadSettings<AppConfig>();
			var (inputFile, outDirectory) = GetInputOutputPaths(config);

			var googleTextToSpeechClient = new GoogleTextToSpeechSynthesizer(inputFile, outDirectory, config.Prepend, Console.WriteLine);
			await googleTextToSpeechClient.Run();

			Console.WriteLine("\nDone!");
			Console.ReadLine();
		}

		private static (string, string) GetInputOutputPaths(AppConfig config)
		{
			if (config.UseDesktop)
			{
				var basePath = Environment.GetFolderPath(SpecialFolder.Desktop);
				return (Path.Combine(basePath, config.InputFileName), Path.Combine(basePath, config.OutputDirectory));
			}
			else
			{
				return (config.InputFileName, config.OutputDirectory);
			}
		}

	}
}

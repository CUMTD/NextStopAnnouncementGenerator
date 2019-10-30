using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace NextStopAnnouncementGenerator.Core
{
	/// <summary>
	/// Synthesizes a CSV containing a collection of T
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Synthesizer<T>
	{
		/// <summary>
		/// The output directory for generated audio files.
		/// </summary>
		protected string OutputDirectory { get; }

		/// <summary>
		/// The path to the input csv file.
		/// </summary>
		protected string InputFile { get; }

		/// <summary>
		/// Action to run for logging.
		/// </summary>
		protected Action<string> LogAction { get; }

		// Delay between synthesis. Useful if synthesis tool is rate limited.
		private int Delay { get; }

		/// <summary>
		/// CsvHelper configuration. Contains sensible defaults but is overridable.
		/// </summary>
		protected virtual Configuration CsvReaderConfiguration => new Configuration(CultureInfo.CurrentCulture)
		{
			BadDataFound = rc => LogAction($"BAD DATA: {rc.RawRecord}"),
			HasHeaderRecord = false,
			IgnoreBlankLines = true,
			IgnoreQuotes = true,
			ReadingExceptionOccurred = re =>
			{
				LogAction($"READING EXCEPTION: {re.Message}");
				return false;
			},
			SanitizeForInjection = true,
			TrimOptions = TrimOptions.Trim
		};

		/// <summary>
		/// Create a new Synthesizer
		/// </summary>
		/// <param name="inputFile">The path to the input csv file.</param>
		/// <param name="outDirectory">The output directory for generated audio files.</param>
		/// <param name="logAction">Action to run for logging.</param>
		/// <param name="delay">Delay between synthesis. Useful if synthesis tool is rate limited.</param>
		protected Synthesizer(string inputFile, string outDirectory, Action<string> logAction = null, int delay = 0)
		{
			InputFile = inputFile ?? throw new ArgumentException(nameof(inputFile));
			OutputDirectory = outDirectory ?? throw new ArgumentException(nameof(outDirectory));
			LogAction = logAction ?? (_str => { });
			Delay = delay;
		}

		/// <summary>
		/// Run the synthesizer.
		/// </summary>
		/// <returns>A task representing the running status.</returns>
		public async Task Run()
		{
			var items = ReadCsv();
			foreach (var item in items)
			{
				await Synth(item);
				LogAction(item.ToString());
				// Allow the thread to sleep in case there is a request limit.
				Thread.Sleep(Delay);
			}
		}

		/// <summary>
		/// Read a CSV file of T.
		/// </summary>
		/// <returns>An array of T containing all items in the CSV file.</returns>
		protected virtual T[] ReadCsv()
		{
			var path = Path.Combine(OutputDirectory, InputFile);
			using var reader = new StreamReader(path);
			using var csv = new CsvReader(reader, CsvReaderConfiguration);
			return csv.GetRecords<T>().ToArray();
		}

		/// <summary>
		/// Synthesize a T.
		/// </summary>
		/// <param name="item">The item to synthesize.</param>
		/// <returns>A task representing the synthesis status.</returns>
		protected abstract Task Synth(T item);
	}

}

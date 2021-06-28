using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
		/// The text to prepend to the each announcement.
		/// </summary>
		protected string Prepend { get; }

		/// <summary>
		/// Action to run for logging.
		/// </summary>
		protected Action<string> LogAction { get; }

		// Delay between synthesis. Useful if synthesis tool is rate limited.
		private int Delay { get; }

		/// <summary>
		/// CsvHelper configuration. Contains sensible defaults but is overridable.
		/// </summary>
		protected virtual CsvConfiguration CsvReaderConfiguration => new(CultureInfo.CurrentCulture)
		{
			BadDataFound = rc => LogAction($"BAD DATA: {rc.RawRecord}"),
			HasHeaderRecord = false,
			IgnoreBlankLines = true,
			ReadingExceptionOccurred = re =>
			{
				LogAction($"READING EXCEPTION: {re.Exception.Message}");
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
		/// <param name="prepend">The text to prepend to each announcement.</param>
		/// <param name="logAction">Action to run for logging.</param>
		/// <param name="delay">Delay between synthesis. Useful if synthesis tool is rate limited.</param>
		protected Synthesizer(string inputFile, string outDirectory, string prepend, Action<string> logAction = null, int delay = 0)
		{
			InputFile = inputFile ?? throw new ArgumentNullException(nameof(inputFile));
			OutputDirectory = outDirectory ?? throw new ArgumentNullException(nameof(outDirectory));
			Prepend = prepend ?? string.Empty;
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
				try
				{
					await Synth(item);
				}
				catch (Exception ex)
				{
					LogAction($"Failed to synth: {ex.Message}");
				}
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

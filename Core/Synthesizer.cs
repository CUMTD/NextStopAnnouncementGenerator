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
		/// The base path for input and output.
		/// </summary>
		protected string BasePath { get; }

		/// <summary>
		/// The name of the CSV input file to read.
		/// </summary>
		protected string InputFileName { get; }

		/// <summary>
		/// Action to run for logging.
		/// </summary>
		protected Action<string> LogAction { get; }
		// Delay between synthesis. Useful if synthesis tool is rate limited.
		private int Delay { get; }

		/// <summary>
		/// CsvHelper configuration. Contains sensible defaults but is overridable.
		/// </summary>
		protected virtual Configuration Configuration => new Configuration(CultureInfo.CurrentCulture)
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
		/// <param name="basePath">The base path for input and output.</param>
		/// <param name="inputFileName">The name of the CSV input file to read.</param>
		/// <param name="logAction">Action to run for logging.</param>
		/// <param name="delay">Delay between synthesis. Useful if synthesis tool is rate limited.</param>
		protected Synthesizer(string basePath, string inputFileName, Action<string> logAction = null, int delay = 0)
		{
			BasePath = basePath ?? throw new ArgumentException(nameof(basePath));
			InputFileName = inputFileName ?? throw new ArgumentException(nameof(inputFileName));
			Delay = delay;
			LogAction = logAction ?? (_str => { });
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
			var path = Path.Combine(BasePath, InputFileName);
			using var reader = new StreamReader(path);
			using var csv = new CsvReader(reader, Configuration);
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

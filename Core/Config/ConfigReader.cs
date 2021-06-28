using System;
using System.IO;
using System.Text.Json;

namespace NextStopAnnouncementGenerator.Core.Config
{
	/// <summary>
	/// Reads JSON config files.
	/// </summary>
	public static class ConfigReader
	{
		/// <summary>
		/// Read settings for a type T.
		/// </summary>
		/// <typeparam name="T">
		/// The type that the settings should be deserialized to.
		/// </typeparam>
		/// <returns>
		/// The deserialized settings object.
		/// </returns>
		/// <remarks>
		/// Will attempt to read the settings from a file with the same name as the type T in the
		/// '\Config' directory.
		/// </remarks>
		public static T ReadSettings<T>()
		{
			var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
			var fileNameBase = $"{typeof(T).Name}";

			var file = new FileInfo(Path.Combine(basePath, $"{fileNameBase}.json"));

#if DEBUG
			var altFile = new FileInfo(Path.Combine(basePath, $"{fileNameBase}.debug.json"));
			if (altFile.Exists)
			{
				file = altFile;
			}
#endif

			if (file.Exists)
			{
				var contents = File.ReadAllText(file.FullName);
				return JsonSerializer.Deserialize<T>(contents, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}

			throw new FileNotFoundException("Can't find config file {0}", file.FullName);
		}
	}
}

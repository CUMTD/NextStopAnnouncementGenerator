using System;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using Console = Colorful.Console;

namespace NextStopAnnouncementGenerator.App
{
	public class VoiceInfo
	{
		private InstalledVoice Voice { get; }
		public string Name => Voice.VoiceInfo.Name;
		public VoiceInfo(InstalledVoice voice)
		{
			Voice = voice ?? throw new ArgumentException(nameof(voice));
		}

		public void Print()
		{
			var info = Voice.VoiceInfo;
			var audioFormats = info
				.SupportedAudioFormats
				.Aggregate(
					new StringBuilder(),
					(sb, af) => sb.AppendLine(af.EncodingFormat.ToString()),
					sb => sb.ToString()
				);

			Console.Write("Name:          ", Color.White);
			Console.WriteLine(info.Name, Color.Gray);
			Console.Write("Culture:       ", Color.White);
			Console.WriteLine(info.Culture, Color.Gray);
			Console.Write("Age:           ", Color.White);
			Console.WriteLine(info.Age, Color.Gray);
			Console.Write("Gender:        ", Color.White);
			Console.WriteLine(info.Gender, Color.Gray);
			Console.Write("Description:   ", Color.White);
			Console.WriteLine(info.Description, Color.Gray);
			Console.Write("ID:            ", Color.White);
			Console.WriteLine(info.Id, Color.Gray);
			Console.Write("Enabled:       ", Color.White);
			Console.WriteLine(Voice.Enabled, Color.Gray);
		}
	}
}

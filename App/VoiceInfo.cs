using System;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;

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

		public override string ToString()
		{
			var output = new StringBuilder();
			var info = Voice.VoiceInfo;
			var audioFormats = info
				.SupportedAudioFormats
				.Aggregate(
					new StringBuilder(),
					(sb, af) => sb.AppendLine(af.EncodingFormat.ToString()),
					sb => sb.ToString()
				);

			output.AppendLine($" Name:          {info.Name}");
			output.AppendLine($" Culture:       {info.Culture}");
			output.AppendLine($" Age:           {info.Age}");
			output.AppendLine($" Gender:        {info.Gender}");
			output.AppendLine($" Description:   {info.Description}");
			output.AppendLine($" ID:            {info.Id}" + info.Id);
			output.AppendLine($" Enabled:       {Voice.Enabled}");

			if (info.SupportedAudioFormats.Count != 0)
			{
				output.AppendLine($" Audio formats: {audioFormats}");
			}
			else
			{
				output.AppendLine(" No supported audio formats found");
			}

			var additionalInfo  = info.AdditionalInfo.Aggregate(
				new StringBuilder(),
				(sb, ai) => sb.AppendFormat("  {0}: {1}\n", ai.Key, ai.Value),
				sb => sb.ToString()
			);

			output.AppendLine($" Additional Info - {additionalInfo}");
			output.AppendLine();

			return output.ToString();
		}
	}
}

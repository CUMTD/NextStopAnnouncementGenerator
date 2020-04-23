using System;
using Google.Cloud.TextToSpeech.V1;

namespace NextStopAnnouncementGenerator.Google.Config
{
	public class GoogleConfig
	{
		public string LanguageCode { get; set; }
		public string Gender { get; set; }
		public string Encoding { get; set; }
		public double SpeakingRate { get; set; }
		public int volumeGainDb { get; set; }
		public SsmlVoiceGender SsmlVoiceGender
		{
			get
			{
				if ("male".Equals(Gender, StringComparison.CurrentCultureIgnoreCase))
				{
					return SsmlVoiceGender.Male;
				}
				if ("female".Equals(Gender, StringComparison.CurrentCultureIgnoreCase))
				{
					return SsmlVoiceGender.Female;
				}
				return "neutral".Equals(Gender, StringComparison.CurrentCultureIgnoreCase) ?
					SsmlVoiceGender.Male :
					SsmlVoiceGender.Unspecified;
			}
		}

		public AudioEncoding AudioEncoding
		{
			get
			{
				if ("Mp3".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return AudioEncoding.Mp3;
				}
				if ("Linear16".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return AudioEncoding.Linear16;
				}
				return "OggOpus".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase) ?
					AudioEncoding.OggOpus :
					AudioEncoding.Unspecified;
			}
		}

		public string FileExtension
		{
			get
			{
				if ("Mp3".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return "mp3";
				}
				if ("Linear16".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return "wav";
				}
				return "OggOpus".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase) ?
					"ogg" :
					"wav";
			}
		}
	}
}

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
		public int VolumeGainDb { get; set; }
		public SsmlVoiceGender SsmlVoiceGender
		{
			get
			{
				if ("male".Equals(Gender, StringComparison.CurrentCultureIgnoreCase))
				{
					return SsmlVoiceGender.Male;
				}
				else if ("female".Equals(Gender, StringComparison.CurrentCultureIgnoreCase) || "neutral".Equals(Gender, StringComparison.CurrentCultureIgnoreCase))
				{
					return SsmlVoiceGender.Female;
				}
				else
				{
					return SsmlVoiceGender.Unspecified;
				}
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
				else if ("Linear16".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return AudioEncoding.Linear16;
				}
				else if ("OggOpus".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return AudioEncoding.OggOpus;
				}
				else
				{
					return AudioEncoding.Unspecified;
				}
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
				else if ("Linear16".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return "wav";
				}
				else if ("OggOpus".Equals(Encoding, StringComparison.CurrentCultureIgnoreCase))
				{
					return "ogg";
				}
				else
				{
					return "wav";
				}
			}
		}
	}
}

using System;
using System.Xml.Linq;
using Google.Cloud.TextToSpeech.V1;

namespace NextStopAnnouncementGenerator.Google.Config
{
	public class GoogleConfig
	{
		public string Encoding { get; set; }
		public double SpeakingRate { get; set; }
		public int VolumeGainDb { get; set; }
		public LanguageOption LanguageOption { get; set; }

		public VoiceSelectionParams VoiceSelectionParams => LanguageOption switch
		{
			LanguageOption.UsMale => new VoiceSelectionParams
			{
				LanguageCode = "en-US",
				SsmlGender = SsmlVoiceGender.Male,
				Name = "en-US-Neural2-D"
			},
			LanguageOption.UkFemale => new VoiceSelectionParams
			{
				LanguageCode = "en-GB",
				SsmlGender = SsmlVoiceGender.Female,
				Name = "en-GB-Standard-A"
			},
			LanguageOption.UkMale => new VoiceSelectionParams
			{
				LanguageCode = "en-GB",
				SsmlGender = SsmlVoiceGender.Male,
				Name = "en-GB-Standard-B"
			},
			_ => new VoiceSelectionParams
			{
				LanguageCode = "en-US",
				SsmlGender = SsmlVoiceGender.Female,
				Name = "en-US-Neural2-F"
			},
		};

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

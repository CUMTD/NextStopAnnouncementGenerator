using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;
using NextStopAnnouncementGenerator.Core;

namespace NextStopAnnouncementGenerator.Google
{

	public class GoogleTextToSpeechSynthesizer : Synthesizer<SsmlStopName>
	{
		private TextToSpeechClient Client { get; }

		public GoogleTextToSpeechSynthesizer(string basePath, string inputFileName, Action<string> logAction) : base(basePath, inputFileName, logAction, 1000)
		{
			var cred = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "googleCreds.json")).ToChannelCredentials();
			var channel = new Channel(TextToSpeechClient.DefaultEndpoint.Host, TextToSpeechClient.DefaultEndpoint.Port, cred);
			Client = TextToSpeechClient.Create(channel);
		}

		private async Task Synth(string fileName, string ssml)
		{

			var input = new SynthesisInput
			{
				Ssml = $"<speak>Now approaching, {ssml}</speak>"
			};

			var voice = new VoiceSelectionParams
			{
				LanguageCode = "en-US",
				SsmlGender = SsmlVoiceGender.Male
			};

			var config = new AudioConfig
			{
				AudioEncoding = AudioEncoding.Mp3,
				SpeakingRate = 0.90,
				VolumeGainDb = 6
			};

			var response = await Client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
			{
				Input = input,
				Voice = voice,
				AudioConfig = config
			})
			.ConfigureAwait(false);

			await using var output = File.Create(Path.Combine(BasePath, fileName));
			response.AudioContent.WriteTo(output);

		}

		#region Synthesizer<SsmlStopName>
		protected override async Task Synth(SsmlStopName item)
		{
			var sanitizedName = SanitizeName(item.StopName);
			var fileName = BuildFileName(sanitizedName);
			var ssml = string.IsNullOrWhiteSpace(item.SsmlOverride) ? sanitizedName : item.SsmlOverride;
			await Synth(fileName, ssml);
		}

		#endregion Synthesizer<SsmlStopName>

		#region Helpers

		private static string SanitizeName(string name)
		{
			var replaced = name.Replace("&", "and");
			var sanitized = Regex.Replace(replaced, @"[^0-9a-zA-Z\s]+", "");
			return sanitized;
		}

		private static string BuildFileName(string sanitizedName) =>
			$"{sanitizedName.Replace(" ", "_").ToLower()}.mp3";

		#endregion Helpers
	}
}

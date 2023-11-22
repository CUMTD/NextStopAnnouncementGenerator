using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NextStopAnnouncementGenerator.Core;
using NextStopAnnouncementGenerator.Core.Config;
using NextStopAnnouncementGenerator.Google.Config;
using Google.Cloud.TextToSpeech.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;

namespace NextStopAnnouncementGenerator.Google
{

	public partial class GoogleTextToSpeechSynthesizer : Synthesizer<SsmlStopName>
	{
		[GeneratedRegex(@"[^0-9a-zA-Z\s]+")]
		private static partial Regex NameReplaceRegex();

		private TextToSpeechClient Client { get; }
		private GoogleConfig GoogleConfig { get; }

		public GoogleTextToSpeechSynthesizer(string inputFile, string outDirectory, string prepend, Action<string> logAction) : base(inputFile, outDirectory, prepend, logAction, 1000)
		{
			var cred = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "googleCreds.json")).ToChannelCredentials();
			Client = new TextToSpeechClientBuilder { ChannelCredentials = cred }.Build();
			GoogleConfig = ConfigReader.ReadSettings<GoogleConfig>();
		}

		private async Task Synth(string fileName, string ssml)
		{
			var input = new SynthesisInput
			{
				Ssml = $"<speak>{Prepend}{ssml}</speak>"
			};

			var voice = GoogleConfig.VoiceSelectionParams;

			var config = new AudioConfig
			{
				AudioEncoding = GoogleConfig.AudioEncoding,
				SpeakingRate = GoogleConfig.SpeakingRate,
				VolumeGainDb = GoogleConfig.VolumeGainDb,
			};

			var response = await Client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
			{
				Input = input,
				Voice = voice,
				AudioConfig = config
			})
			.ConfigureAwait(false);

			await using var output = File.Create(Path.Combine(OutputDirectory, fileName));
			response.AudioContent.WriteTo(output);

		}

		#region Synthesizer<SsmlStopName>
		protected override async Task Synth(SsmlStopName item)
		{
			var sanitizedName = SanitizeName(item.StopName);
			var fileName = BuildFileName(sanitizedName, GoogleConfig.FileExtension);
			var ssml = string.IsNullOrWhiteSpace(item.SsmlOverride) ? sanitizedName : item.SsmlOverride;
			await Synth(fileName, ssml);
		}

		#endregion Synthesizer<SsmlStopName>

		#region Helpers

		private static string SanitizeName(string name)
		{
			var replaced = name.Replace("&", "and");
			var sanitized = NameReplaceRegex().Replace(replaced, "");
			return sanitized;
		}

		private static string BuildFileName(string sanitizedName, string fileExtension) =>
			$"{sanitizedName.Replace(" ", "_").ToLower()}.{fileExtension}";
		

		#endregion Helpers
	}
}

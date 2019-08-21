using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;

namespace NextStopAnnouncementGenerator.Google
{

	public class GoogleTextToSpeechClient
	{
		private TextToSpeechClient Client { get; }

		private string BasePath { get; }

		public GoogleTextToSpeechClient(string basePath)
		{
			BasePath = basePath ?? throw new ArgumentException(nameof(basePath));

			var cred = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "googleCreds.json"));
			var channel = new Channel(
				TextToSpeechClient.DefaultEndpoint.Host, TextToSpeechClient.DefaultEndpoint.Port, cred.ToChannelCredentials());

			Client = TextToSpeechClient.Create(channel);
		}

		private Task Synth(string text) => Synth(text, SanitizeName(text));

		private async Task Synth(string name, string ssml)
		{

			var input = new SynthesisInput
			{
				Ssml = $"<speak>Now approaching, {ssml}</speak>"
			};
			await Synth(BuildFileName(SanitizeName(name)), input).ConfigureAwait(false);
		}

		private async Task Synth(string fileName, SynthesisInput input)
		{
			var voice = new VoiceSelectionParams
			{
				LanguageCode = "en-US",
				SsmlGender = SsmlVoiceGender.Female
			};

			var config = new AudioConfig
			{
				AudioEncoding = AudioEncoding.Mp3,
				SpeakingRate = 0.90
			};

			var response = await Client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
			{
				Input = input,
				Voice = voice,
				AudioConfig = config
			})
			.ConfigureAwait(false);

			using (var output = File.Create(Path.Combine(BasePath, fileName)))
			{
				response.AudioContent.WriteTo(output);
			}
		}

		public async Task Run(Action<string> logAction = null)
		{
			var path = Path.Combine(BasePath, "next_stop.csv");
			var lines = File.ReadAllLines(path);
			foreach (var line in lines)
			{
				var splits = line.Split(',').Where(l=>!string.IsNullOrEmpty(l)).ToArray();
				if (splits.Length > 1)
				{
					await Synth(splits[0], splits[1]).ConfigureAwait(false);
				}
				else
				{
					await Synth(splits[0]);
				}
				logAction?.Invoke(line);
				// sleep so we don't go over google's limit
				Thread.Sleep(1000); 
			}
		}

		private static string SanitizeName(string name)
		{
			var replaced = name.Replace("&", "and");
			var sanitized = Regex.Replace(replaced, @"[^0-9a-zA-Z\s]+", "");
			return sanitized;
		}

		private static string BuildFileName(string sanitizedName) =>
			$"{sanitizedName.Replace(" ", "_").ToLower()}.mp3";
	}
}

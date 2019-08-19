using System;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace NextStopAnnouncementGenerator.App
{
	public class Synth
	{
		private int _currentVoiceIndex = 0;
		public VoiceInfo CurrentVoice => Voices[_currentVoiceIndex];
		public VoiceInfo[] Voices { get; }

		private static readonly SpeechAudioFormatInfo formatInfo = new SpeechAudioFormatInfo(
			32_000,
			AudioBitsPerSample.Sixteen,
			AudioChannel.Mono
		);

		public Synth()
		{
			using (var synth = new SpeechSynthesizer())
			{
				Voices = synth
					.GetInstalledVoices()
					.Select(v => new VoiceInfo(v))
					.ToArray();
			}
		}

		private SpeechSynthesizer GetSynth(string outFile)
		{
			var synth = new SpeechSynthesizer();
			synth.SelectVoice(CurrentVoice.Name);
			synth.SetOutputToWaveFile(outFile, formatInfo);
			return synth;
		}
		private static string SanatizeName(string name)
		{
			var replaced = name.Replace("&", "and");
			var sanatized = Regex.Replace(replaced, @"[^0-9a-zA-Z\s]+", "");
			return sanatized;
		}

		private static string BuildFileName(string sanatizedName) =>
			$"{sanatizedName.Replace(" ", "_").ToLower()}.wav";


		private void CreateWavFile(string outFolder, string value, string prefix = null, Action<string> loggingCallback = null)
		{
			var sanatized = SanatizeName(value);
			var fileName = BuildFileName(sanatized);
			using (var synth = GetSynth(Path.Combine(outFolder, fileName)))
			{
				var speakValue = string.IsNullOrWhiteSpace(prefix) ? value : $"{prefix} {value}";
				synth.Speak(speakValue);
				loggingCallback?.Invoke(speakValue);
			}
		}

		public void Run(string basePath)
		{
			var path = Path.Combine(basePath, "next_stop.txt");
			var lines = File.ReadAllLines(path);
			foreach (var line in lines)
			{
				CreateWavFile(basePath, line, "Now approaching,", Console.WriteLine);
			}
		}

		public void ChangeVoice(int newVoiceIndex) => _currentVoiceIndex = newVoiceIndex;
		public void ListVoices()
		{
			for(var i = 0; i < Voices.Length; i++)
			{
				Console.WriteLine($"{i+1}: {Voices[i].Name}");
			}
		}

	}
}

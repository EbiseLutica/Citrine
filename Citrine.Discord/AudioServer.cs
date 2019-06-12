using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Newtonsoft.Json;

namespace Citrine.Discord
{
	public sealed class AudioServer : IDisposable
	{
		public ConcurrentQueue<MusicInfo> AudioQueue { get; } = new ConcurrentQueue<MusicInfo>();

		public MusicInfo CurrentAudio { get; private set; }

		public IMessageChannel LogChannel { get; set; }

		public AudioServer(Shell s)
		{
			shell = s;
			ctsToDispose = new CancellationTokenSource();
			worker = Task.Factory.StartNew((_) => AudioWorker(), ctsToDispose, TaskCreationOptions.LongRunning);
		}

		public async Task SummonAsync(IVoiceChannel ch)
		{
			channel?.DisconnectAsync();
			cli?.Dispose();

			cli = await ch.ConnectAsync();
			channel = ch;
		}

		public async Task<MusicInfo> PlayAsync(string url, Citrine.Core.Api.IUser addedBy)
		{
			var aud = await GetMusicInfoAsync(url);
			aud.AddedBy = addedBy;
			AudioQueue.Enqueue(aud);
			return aud;
		}

		public void Skip()
		{
			ctsToSkip?.Cancel();
		}

		public void Clear()
		{
			AudioQueue.Clear();
			Skip();
		}

		public async Task ChangeCurrentTextChannelAsync(string name)
		{
			var tcs = await channel.Guild.GetTextChannelsAsync();
			var tc = tcs.FirstOrDefault(c => c.Name == name);
			if (tc == null)
				throw new ArgumentException(nameof(name));
			LogChannel = tc;
		}

		public async Task KillAsync()
		{
			await channel?.DisconnectAsync();
		}

		public async Task<Process> FFMpegAsync(Process proc)
		{
			using (proc)
			{
				var ffmpeg = await ProcessStartAsync(new ProcessStartInfo
				{
					FileName = "ffmpeg",
					Arguments = "-hide_banner -loglevel quiet -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true
				}, proc.StandardOutput.BaseStream);
				return ffmpeg;
			};
		}

		public Task<Process> YoutubeDlAsync(string url)
		{
			return ProcessStartAsync(new ProcessStartInfo
			{
				FileName = "youtube-dl",
				Arguments = $"-q \"{url}\" -o -",
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true
			});
		}

		public async Task<MusicInfo> GetMusicInfoAsync(string url)
		{
			var str = await ProcessStartAndReadStandardOutputAsStringAsync(new ProcessStartInfo
			{
				FileName = "youtube-dl",
				Arguments = $"--dump-json \"{url}\" -q -o -",
				UseShellExecute = false,
				RedirectStandardOutput = true,
			});
			Console.WriteLine(str);
			return JsonConvert.DeserializeObject<MusicInfo>(str);
		}

		public async Task<string> ProcessStartAndReadStandardOutputAsStringAsync(ProcessStartInfo psi)
		{
			using (var proc = await ProcessStartAsync(psi))
			using (var stream = proc.StandardOutput)
			{
				return await stream.ReadToEndAsync();
			}
		}

		public Task<Process> ProcessStartAsync(ProcessStartInfo psi, Stream stream = null)
		{
			return Task.Run<Process>(async () =>
			{
				var proc = Process.Start(psi);
				if (stream != null)
				{
					proc.StandardInput.AutoFlush = true;
					await stream.CopyToAsync(proc.StandardInput.BaseStream);
				};
				proc.WaitForExit();
				return proc;
			});
		}

		private async Task AudioWorker()
		{
			while (!ctsToDispose.Token.IsCancellationRequested)
			{
				await StreamingAsync();
			}
		}

		private async Task StreamingAsync()
		{
			if (AudioQueue.Count == 0)
				return;
			ctsToSkip = new CancellationTokenSource();
			if (AudioQueue.TryDequeue(out var aud))
			{
				CurrentAudio = aud;
				using (var ytdl = await YoutubeDlAsync(CurrentAudio.WebpageUrl))
				using (var ffmpeg = await FFMpegAsync(ytdl))
				using (var stream = cli?.CreatePCMStream(AudioApplication.Music))
				{
					await LogChannel?.SendMessageAsync($"🎶**「{CurrentAudio.Title}」を再生開始するよ〜.**");
					try
					{
						await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream, ctsToSkip.Token);
					}
					finally
					{
						await stream.FlushAsync();
					}
				}
			}
			CurrentAudio = null;
		}

		public void Dispose()
		{
			cli?.Dispose();
			channel?.DisconnectAsync();
			ctsToSkip?.Cancel();
			ctsToDispose?.Cancel();
		}

		Shell shell;
		IVoiceChannel channel;
		IAudioClient cli;
		CancellationTokenSource ctsToDispose = new CancellationTokenSource();
		CancellationTokenSource ctsToSkip;
		Task worker;
	}

	// duration, fulltitle, description, uploader, thumbnail, webpage_url
	public class MusicInfo
	{
		[JsonProperty("duration")]
		public int Duration { get; set; }
		[JsonProperty("fulltitle")]
		public string Title { get; set; }
		[JsonProperty("description")]
		public string Description { get; set; }
		[JsonProperty("uploader")]
		public string Uploader { get; set; }
		[JsonProperty("thumbnail")]
		public string ThumbnailUrl { get; set; }
		[JsonProperty("webpage_url")]
		public string WebpageUrl { get; set; }
		public Citrine.Core.Api.IUser AddedBy { get; set; }
	}
}

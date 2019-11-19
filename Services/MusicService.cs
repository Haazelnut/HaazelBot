using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Haazelbot.Services
{
    public class MusicService
    {
        private LavaNode _lavaNode;
        private DiscordSocketClient _client;
        private LavaPlayer _player;

        public MusicService(LavaNode lavaNode, DiscordSocketClient client)
        {
            _lavaNode = lavaNode;
            _client = client;
        }

        public Task InitializeAsync()
        {
            _client.Ready += OnReadyAsync;
            _lavaNode.OnLog += LogAsync;
            _lavaNode.OnTrackEnded += TrackEnded;

            return Task.CompletedTask;
        }

        public async Task OnReadyAsync()
        {

            LavaConfig config = new LavaConfig()
            {
                LogSeverity = LogSeverity.Verbose,
            };

            await _lavaNode.ConnectAsync();
        }

        public async Task JoinChannelAsync(SocketVoiceChannel voiceChannel, SocketTextChannel textChannel)
        => await _lavaNode.JoinAsync(voiceChannel, textChannel);

        public async Task LeaveChannelAsync(SocketVoiceChannel voiceChannel)
        => await _lavaNode.LeaveAsync(voiceChannel);

        public async Task<string> PlayAsync(string query, IGuild guild)
        {
            _player = _lavaNode.GetPlayer(guild);
            var results = await _lavaNode.SearchYouTubeAsync(query);

            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                return "**No matches found.**";
            }

            var track = results.Tracks.FirstOrDefault();

            if (_player.PlayerState == PlayerState.Playing)
            {
                _player.Queue.Enqueue(track);
                return $"**{track.Title} has been added to the queue**";
            }
            else
            {
                await _player.PlayAsync(track);
                await _client.SetGameAsync(track.Title);
                return $":ok_hand::rofl:**Now playing {track.Title}**:rofl::ok_hand:";
            }
        }

        public async Task<string> StopAsync()
        {
            if (_player is null)
                return "Player is not working";

            await _player.StopAsync();
            return "**Stopped playing music**";
        }

        public async Task<string> SkipAsync()
        {
            if (_player is null)
                return "**Player is not working**";

            if (_player.Queue.Count is 0)
            {
                var oldTrack = _player.Track;
                await _player.StopAsync();
                return $"**Skipped {oldTrack.Title}**";
            }
            else
            {
                var oldTrack = _player.Track;
                await _player.SkipAsync();
                await _client.SetGameAsync(_player.Track.Title);
                return $"**Skipped {oldTrack.Title}** \n**Now Playing: {_player.Track.Title}**";
            }
        }

        public async Task<string> SetVolumeAsync(ushort vol)
        {
            if (_player is null)
                return "**Player is not working**";

            if (vol > 150 || vol <= 2)
                return "**Please use a number between 2 and 150**";

            await _player.UpdateVolumeAsync(vol);
            return $"**Volume set to {vol}**";
        }

        public async Task<string> PauseAsync()
        {
            if (_player is null)
                return "**Player is not working**";

            if (_player.PlayerState == PlayerState.Paused)
                return "**Music is already paused**";

            await _player.PauseAsync();
            return "**Paused the music**";
        }

        public async Task<string> ResumeAsync()
        {
            if (_player is null)
                return "**Player is not working**";

            if (_player.PlayerState == PlayerState.Playing)
                return "**Music is already playing**";

            await _player.ResumeAsync();
            return $"**Resumed: {_player.Track.Title}**";
        }


        private async Task TrackEnded(TrackEndedEventArgs arg)
        {
            if (!arg.Reason.ShouldPlayNext())
                return;

            if (arg.Player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await _client.SetGameAsync("");
                return;
            }

            await arg.Player.PlayAsync(nextTrack);
            await _client.SetGameAsync(nextTrack.Title);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }


    }
}

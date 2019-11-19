using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Haazelbot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace Haazelbot.Commands
{
    public class Music : ModuleBase<SocketCommandContext>
    {

        private MusicService _musicService;

        public Music(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("Join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;

            if (user.VoiceChannel is null)
            {
                await ReplyAsync("**You need to connect to a voice channel.**");
                return;
            }
            else
            {
                await _musicService.JoinChannelAsync(user.VoiceChannel, Context.Channel as SocketTextChannel);
                await ReplyAsync($"**Connected to {user.VoiceChannel}**");
            }
        }
        
        [Command("Leave")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;

            if (user.VoiceChannel is null)
            {
                await ReplyAsync("**Please join the channel that the bot is in to make it leave**");
            }
            else
            {
                await _musicService.LeaveChannelAsync(user.VoiceChannel);
            }
            
        }

        [Command("Play")]
        public async Task Play([Remainder]string query)
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("**You need to connect to a voice channel.**");
                return;
            }
            else
            {
                await _musicService.JoinChannelAsync(user.VoiceChannel, Context.Channel as SocketTextChannel);
                string result = await _musicService.PlayAsync(query, Context.Guild);
                await ReplyAsync(result);
            }
            
        }

        [Command("Stop")]
        public async Task Stop()
            => await ReplyAsync(await _musicService.StopAsync());

        [Command("Skip")]
        public async Task Skip()
        => await ReplyAsync(await _musicService.SkipAsync());

        [Command("Volume")]
        public async Task Volume(ushort vol)
        => await ReplyAsync(await _musicService.SetVolumeAsync(vol));

        [Command("Pause")]
        public async Task Pause()
        => await ReplyAsync(await _musicService.PauseAsync());

        [Command("Resume")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync());
    }
}

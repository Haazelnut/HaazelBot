using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Haazelbot.Commands;
using Haazelbot.Data;
using Haazelbot.Guilds;
using Haazelbot.LevelingSystem;
using Haazelbot.Services;
using Haazelbot.Users;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace Haazelbot
{
    public class Program
    {

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commandService;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            //Process process = new Process();
            //string lavaLinkFileLocation = Environment.GetEnvironmentVariable("LavaLinkFileLocation");
            //process.StartInfo = new ProcessStartInfo(@"java.exe", $"-jar {lavaLinkFileLocation}")
            //{
            //    UseShellExecute = false,
            //    CreateNoWindow = true
            //};

            //try
            //{
            //    process.Start();
            //}
            //catch (System.ComponentModel.Win32Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            

            using (var context = new ApplicationDbContext())
            {
                context.Database.EnsureCreated();
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
            });

            _commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,

            });

            _services = SetupServices();

            string botToken = Environment.GetEnvironmentVariable("BotToken");

            _client.Log += Log;
            _client.UserJoined += UserJoinedAnnouncement;
            _client.JoinedGuild += OnGuildJoin;
            //_client.LeftGuild += OnGuildLeave;
            //_client.Ready += OnBotConnection;
            CommandHandler cmdHandler = new CommandHandler(_client, _commandService, _services);

            await cmdHandler.InitializeAsync();

            await _services.GetRequiredService<MusicService>().InitializeAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices()
            => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commandService)
            .AddSingleton<LavaConfig>()
            .AddSingleton<LavaNode>()
            .AddSingleton<MusicService>()
            .BuildServiceProvider();

        private async Task UserJoinedAnnouncement(SocketGuildUser user)
        {
            Accounts.GetAccount(user, user.Guild.Id);
            var guild = user.Guild;
            var channel = guild.DefaultChannel;
            await channel.SendMessageAsync($"Hello {user.Mention}");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }


        private Task OnGuildJoin(SocketGuild guild)
        {
            IReadOnlyCollection<SocketGuildUser> users = guild.Users;
            foreach (var user in users)
            {
                Accounts.GetAccount(user, guild.Id);
            }

            return Task.CompletedTask;
        }

        private Task OnBotConnection()
        {
            var guilds = _client.Guilds.ToList();

            GuildController.SaveGuilds(guilds);

            foreach (var guild in guilds)
            {
                var users = guild.Users.ToList();

                Accounts.SaveAccounts(users);
            }

            return Task.CompletedTask;
        }

        //private async Task OnGuildLeave(SocketGuild guild)
        //{

        //}
    }
}

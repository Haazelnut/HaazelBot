using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Haazelbot.LevelingSystem;
using Haazelbot.Users;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Haazelbot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services)
        {
            _client = client;
            _commandService = commandService;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _commandService.Log += LogAsync;
            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message) || message.Author.IsBot) return;
            var context = new SocketCommandContext(_client, message);
            var userAccount = Accounts.GetAccount(context.User, context.Guild.Id);
            if (userAccount.IsMuted)
            {
                await context.Message.DeleteAsync();
                return;
            }

            int argPos = 0;

            if (message.HasStringPrefix("%", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {

                var result = await _commandService.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
            else
            {
                //Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);
            }

        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}

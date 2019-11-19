using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Haazelbot.Users;

namespace Haazelbot.LevelingSystem
{
    internal static class Leveling
    {

        internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
        {
            var userAccount = Accounts.GetAccount(user, channel.Guild.Id);


            uint oldLevel = userAccount.Level;
            userAccount.XP += 50;
            Accounts.SaveAccounts();

            if (oldLevel != userAccount.Level)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(67, 160, 71);
                embed.WithTitle("Level Up!");
                embed.WithDescription($"{user.Username} just leveled up!");
                embed.AddField("LEVEL", userAccount.Level);

                await channel.SendMessageAsync("", embed: embed.Build());
            }
        }
    }
}

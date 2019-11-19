using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Haazelbot.UserAccounts;
using Newtonsoft.Json;
using Haazelbot.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haazelbot.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("Help")]
        [Alias("Commands")]
        public async Task Help()
        {
            var embed = new EmbedBuilder();
            embed.WithColor(67, 160, 71);
            embed.WithTitle("Commands");
            embed.WithDescription($"Here are the commands for the bot, parameters between () is optional");
            embed.AddField("%WhatLvlIs x", "Show the level for the given XP x");
            embed.AddField("%ShowXP (@user) or %XP (@user)", "Shows the XP of you or the mentioned target");
            embed.AddField("%ResetXP (@user)", "Resets the XP of you or the mentioned target");
            embed.AddField("%Warn @user", "Warns the mentioned target (10 warnings = kick, 25 warnings = ban)");
            embed.AddField("%Mute @user", "Mutes the mentioned target");
            embed.AddField("%Unmute @user", "Unmutes the mentioned target");
            embed.AddField("%Kick @user", "Kicks the mentioned target");
            embed.AddField("%Ban @user", "Bans the mentioned target");
            embed.AddField("%Unban userID", "Unbans the user with the given UserId");
            embed.AddField("%Purge (x) or %Clear (x) or %Delete (x)", "Deletes x amount of messages (x can be a maximum of 100 and default is 1)");

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("WhatLvlIs")]
        public async Task WhatLvlIs(uint xp)
        {
            uint level = (uint)Math.Sqrt(xp / 50);
            await Context.Channel.SendMessageAsync("The level is " + level);

        }

        [Command("ShowXP")]
        [Alias("xp")]
        public async Task ShowXP([Remainder] string arg = "")
        {

            SocketUser target;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var account = Accounts.GetAccount(target, Context.Guild.Id);
            await Context.Channel.SendMessageAsync($"{target.Username} is lvl {account.Level} and have {account.XP} XP and {account.Points} points");
        }

        [Command("ResetXP")]
        public async Task ResetXP([Remainder] string arg = "")
        {
            SocketUser target;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var user = Context.User as SocketGuildUser;
            var targetUser = Accounts.GetAccount(target, Context.Guild.Id);
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Admin");
            if (target == mentionedUser && user.Roles.Contains(role))
            {
                targetUser.XP = 0;
                await Context.Channel.SendMessageAsync($"You have reset {target.Username}' xp");
            }
            else if (target == Context.User)
            {
                targetUser.XP = 0;
                await Context.Channel.SendMessageAsync($"You have reset your own xp");
            }
            else
            {

                await Context.Channel.SendMessageAsync($"You do not have the right permissions to reset {target.Username}' xp");
            }
        }

        [Command("AddXP")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddXP(uint xpAmount, [Remainder] string arg = "")
        {

            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            target = mentionedUser ?? Context.User;

            var account = Accounts.GetAccount(target, Context.Guild.Id);

            account.XP += xpAmount;
            await Context.Channel.SendMessageAsync($"{target.Username} gained {xpAmount} XP. ");

            Accounts.SaveAccounts();
        }

        [Command("id"), RequireOwner]
        public async Task MentionAsync(SocketGuildUser user)
        {
            await ReplyAsync($"{user.Username} has the id: {user.Id.ToString()}");
        }

        [Command("Warn")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task WarnUser(IGuildUser user)
        {
            var userAccount = Accounts.GetAccount((SocketUser)user, Context.Guild.Id);

            userAccount.NumberOfWarnings++;
            Accounts.SaveAccounts();

            if (userAccount.NumberOfWarnings >= 25)
            {
                await BanUser(user, 1, "You have been banned from the server");
                await Context.Channel.SendMessageAsync($"{user.Username} with the ID {user.Id} was banned from the server due to having a total of {userAccount.NumberOfWarnings} warnings");
            }
            else if (userAccount.NumberOfWarnings == 10)
            {
                await KickUser(user);
                await Context.Channel.SendMessageAsync($"{user.Username} was kicked from the server due to having a total of {userAccount.NumberOfWarnings} warnings");
            }
            else if (userAccount.NumberOfWarnings == 1)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Username} has warned {user.Username} and now has a total of {userAccount.NumberOfWarnings} warnings");
            }

        }

        [Command("Mute")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MuteUser(IGuildUser user)
        {
            if (!user.IsBot)
            {
                var account = Accounts.GetAccount((SocketUser)user, Context.Guild.Id);

                account.IsMuted = true;
                Accounts.SaveAccounts();

                await Context.Channel.SendMessageAsync($"{Context.User} has muted {user.Username}");
            }
        }

        [Command("Unmute")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UnmuteUser(IGuildUser user)
        {
            if (!user.IsBot)
            {
                var account = Accounts.GetAccount((SocketUser)user, Context.Guild.Id);

                account.IsMuted = false;
                Accounts.SaveAccounts();

                await Context.Channel.SendMessageAsync($"{Context.User} has unmuted {user.Username}");
            }
        }

        [Command("Kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, string reason = "You got kicked from the server")
        {
            if (!user.IsBot)
            {
                await user.KickAsync(reason);
            }
        }

        [Command("Ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, int length = 1, string reason = "You got banned from the server")
        {
            if (user.Username != "Haazelbot"/*!user.IsBot*/)
            {
                await user.Guild.AddBanAsync(user, length, reason);
                await Context.Channel.SendMessageAsync($"{Context.User} has banned {user.Username} with UserID: {user.Id} for {length} days");
            }
        }

        [Command("Unban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task UnbanUser(ulong userId)
        {
            await Context.Guild.RemoveBanAsync(userId);
            await Context.Channel.SendMessageAsync($"{Context.User} has unbanned the id {userId} from the server");
        }

        [Command("BannedUsers")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BannedUsers()
        {
            IReadOnlyCollection<Discord.Rest.RestBan> bannedUsers = await Context.Guild.GetBansAsync();

            var embed = new EmbedBuilder();
            embed.WithColor(67, 160, 71);
            embed.WithTitle("Banned Users");

            if (bannedUsers.Count == 0)
            {
                embed.AddField("------------", "No banned users on this server");
            }
            else
            {
                foreach (var ban in bannedUsers)
                {
                    embed.AddField(ban.User.ToString(), ban.Reason);
                }
            }
            

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("purge")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Alias("clear", "delete")]
        public async Task Purge([Remainder] int num = 0)
        {
            if (num <= 100)
            {
                var messagesToDelete = await Context.Channel.GetMessagesAsync(num + 1).FlattenAsync();
                int DeletedMessages = 0;
                foreach (var item in messagesToDelete)
                {
                    DeletedMessages++;
                    await Context.Channel.DeleteMessageAsync(item);

                }
                await Context.Channel.SendMessageAsync(Context.User.Username + " deleted " + DeletedMessages + " messages.");
            }
            else
            {
                await ReplyAsync("You cannot delete more than 100 messages");
            }
        }
    }
}

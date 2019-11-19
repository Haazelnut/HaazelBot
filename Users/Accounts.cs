using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Haazelbot.Data;
using Haazelbot.UserAccounts;
using System.IO;
using Haazelbot.Guilds;

namespace Haazelbot.Users
{
    public static class Accounts
    {
        private static List<UserAccount> accounts;

        static Accounts()
        {
            if (accounts is null)
            {
                accounts = new List<UserAccount>();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts);
        }

        public static void SaveAccounts(List<SocketGuildUser> guildUsers)
        {
            foreach (var user in guildUsers)
            {
                CreateUserAccount(user, user.Guild.Id);
            }
        }

        public static UserAccount GetAccount(SocketUser user, ulong guildId)
        {
            var result = from a in accounts
                         where a.ID == user.Id
                         select a;

            UserAccount account = result.FirstOrDefault();
            if (account is null)
            {
                account = CreateUserAccount(user, guildId);
            }
            return account;
            
        }

        public static UserAccount GetUserByName(string userName)
        {
            var result = from a in accounts
                         where a.Name == userName
                         select a;

            UserAccount account = result.FirstOrDefault();
            return account;
        }

        private static UserAccount CreateUserAccount(SocketUser user, ulong guildId)
        {
            string userName = user.ToString();
            UserAccount newAccount = new UserAccount()
            {
                ID = user.Id,
                Name = userName,
                Points = 0,
                XP = 0,
                IsMuted = false,
                NumberOfWarnings = 0,
                GuildId = guildId

            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }

    }
}

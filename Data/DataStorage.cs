using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Haazelbot.UserAccounts;
using System.IO;
using Haazelbot.Guilds;

namespace Haazelbot.Data
{
    public static class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts)
        {
            using (var con = new ApplicationDbContext())
            {
                foreach (var account in accounts)
                {
                    var accountTemp = con.Accounts.Find(account.ID);
                    if (accountTemp == null)
                    {
                        con.Accounts.Add(account);
                    }
                }

                con.SaveChanges();
            }
        }

        public static IEnumerable<UserAccount> LoadUserAccounts()
        {
            using (var con = new ApplicationDbContext())
            {
                return con.Accounts.ToArray();
            }
        }

        public static void SaveGuilds(IEnumerable<Guild> guilds)
        {
            using (var con = new ApplicationDbContext())
            {
                foreach (var guild in guilds)
                {
                    var guildTemp = con.Guilds.Find(guild.ID);
                    if (guildTemp == null)
                    {
                        con.Guilds.Add(guild);
                    }
                }

                con.SaveChanges();
            }
        }

        public static IEnumerable<Guild> LoadGuildSettings()
        {
            using (var con = new ApplicationDbContext())
            {
                return con.Guilds.ToArray();
            }
        }
    }
}

using Discord.WebSocket;
using Haazelbot.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Haazelbot.Guilds
{
    public static class GuildController
    {

        private static List<Guild> guilds;

        static GuildController()
        {
            if (guilds == null)
            {
                guilds = new List<Guild>();
            }
        }

        public static void SaveGuilds()
        {
            DataStorage.SaveGuilds(guilds);
        }

        public static void SaveGuilds(List<SocketGuild> guilds)
        {
            foreach (var guild in guilds)
            {
                CreateGuild(guild);
            }
        }

        private static Guild CreateGuild(SocketGuild guild)
        {
            var newGuild = new Guild()
            {
                ID = guild.Id,
                Name = guild.Name,
            };

            guilds.Add(newGuild);
            SaveGuilds();
            return newGuild;
        }

        //public static GuildSettings LoadGuildSettings(string filePath)
        //{

        //}
    }
}

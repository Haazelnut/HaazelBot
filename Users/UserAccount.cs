using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Haazelbot.UserAccounts
{
    public class UserAccount
    {
        public ulong ID { get; set; }

        public string Name { get; set; }

        public uint Points { get; set; }

        public uint XP { get; set; }

        public uint Level
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }

        public bool IsMuted { get; set; }

        public uint NumberOfWarnings { get; set; }

        public virtual ulong GuildId { get; set; }


    }
}

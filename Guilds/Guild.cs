using Haazelbot.UserAccounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haazelbot.Guilds
{
    public class Guild
    {
        public ulong ID { get; set; }

        public string Name { get; set; }

        public virtual IList<UserAccount> Accounts { get; set; }
    }
}

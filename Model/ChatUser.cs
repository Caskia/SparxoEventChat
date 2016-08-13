using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparxoEventChat.Model
{
    public class ChatUser
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public IList<string> Rooms { get; set; } = new List<string>();
    }
}

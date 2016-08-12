using SparxoEventChat.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparxoEventChat.Repository
{
    public class InMemoryRepository
    {
        #region Fields
        private static InMemoryRepository _instance = null;
        private readonly ICollection<ChatUser> _connectedUsers = new List<ChatUser>();
        private readonly ConcurrentDictionary<string, string> _mappings = new ConcurrentDictionary<string, string>();
        private readonly int max_random = 3;
        #endregion

        #region Ctor
        public InMemoryRepository()
        {
            
        }
        public static InMemoryRepository GetInstance()
        {
            if (_instance == null)
            {
                _instance = new InMemoryRepository();
            }
            return _instance;
        }
        #endregion

        #region Methods
        public IQueryable<ChatUser> Users { get { return _connectedUsers.AsQueryable(); } }

        public void Add(ChatUser user)
        {
            _connectedUsers.Add(user);
        }

        public void Remove(ChatUser user)
        {
            _connectedUsers.Remove(user);
        }

        public string GetRandomizedUsername(string username)
        {
            string tempUsername = username;
            int newRandom = max_random, oldRandom = 0;
            int loops = 0;
            Random random = new Random();
            do
            {
                if (loops > newRandom)
                {
                    oldRandom = newRandom;
                    newRandom *= 2;
                }
                username = tempUsername + "_" + random.Next(oldRandom, newRandom).ToString();
                loops++;
            } while (GetInstance().Users.Where(u => u.Username.Equals(username)).ToList().Count > 0);

            return username;
        }

        public bool AddMapping(string connectionId, string userId)
        {
            if (!string.IsNullOrEmpty(connectionId) && !string.IsNullOrEmpty(userId))
            {
                return _mappings.TryAdd(connectionId, userId);
            }
            return false;
        }

        public bool RemoveMapping(string connectionId)
        {
            string userId;
            return _mappings.TryRemove(connectionId,out userId);
        }

        public string GetUserByConnectionId(string connectionId)
        {
            string userId = null;
            _mappings.TryGetValue(connectionId, out userId);
            return userId;
        }

        public ChatUser GetChatUserByConnectionId(string connectionId)
        {
            var userId = GetUserByConnectionId(connectionId);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = Users.Where(u => u.Id == userId).FirstOrDefault();
                return user;
            }
            return null;
        }

        #endregion
    }
}

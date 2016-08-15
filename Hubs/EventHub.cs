using Microsoft.AspNetCore.SignalR;
using SparxoEventChat.Model;
using SparxoEventChat.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparxoChat.Hubs
{
    public class EventHub : Hub
    {
        #region Fields

        private readonly List<string> _eventChatRooms = new List<string>();
        private InMemoryRepository _repository;

        #endregion Fields

        #region Ctor

        public EventHub()
        {
            _repository = InMemoryRepository.GetInstance();
            InitEventChatRooms();
        }

        #endregion Ctor

        public async Task JoinEventChatRoom(string roomName)
        {
            if (!_eventChatRooms.Contains(roomName))
            {
                throw new Exception($"Room[{roomName}] not found! ");
            }
            var user = _repository.GetChatUserByConnectionId(Context.ConnectionId);
            if (user == null)
            {
                throw new Exception("You should logon! ");
            }
            if (!user.Rooms.Contains(roomName))
            {
                user.Rooms.Add(roomName);
            }
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).newUserJoinedEventChatRoom(roomName, user.Username);
        }

        public List<string> LoadEventChatRooms()
        {
            return _eventChatRooms;
        }

        public void LogOn(string username)
        {
            ChatUser user = new ChatUser()
            {
                Id = Guid.NewGuid().ToString(),
                Username = username
            };
            _repository.Add(user);
            _repository.AddMapping(Context.ConnectionId, user.Id);
            Clients.All.joins(user.Id, username, DateTime.Now);
        }

        public void SendMessageInRoom(string roomName, string message)
        {
            var user = _repository.GetChatUserByConnectionId(Context.ConnectionId);
            if (user == null)
            {
                throw new Exception("You should logon! ");
            }
            Clients.Group(roomName).roomMessage(roomName, user.Username, message);
        }

        #region Connect Event Handler Methods

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = _repository.GetChatUserByConnectionId(Context.ConnectionId);
            if (user != null)
            {
                _repository.Remove(user);
                _repository.RemoveMapping(Context.ConnectionId);
                Clients.Groups(user.Rooms).leaves(user.Id, user.Username, DateTime.Now);
            }
            return base.OnDisconnected(stopCalled);
        }

        #endregion Connect Event Handler Methods

        #region Utilities

        private void InitEventChatRooms()
        {
            _eventChatRooms.Add("1");
            _eventChatRooms.Add("2");
            _eventChatRooms.Add("3");
            _eventChatRooms.Add("4");
            _eventChatRooms.Add("5");
        }

        #endregion Utilities
    }
}
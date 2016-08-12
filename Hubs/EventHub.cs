using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using SparxoEventChat.Repository;
using SparxoEventChat.Model;

namespace SparxoChat.Hubs
{
    public class EventHub : Hub
    {
        #region Fields
        private readonly List<string> _eventChatRooms = new List<string>();
        private InMemoryRepository _repository;
        #endregion

        #region Ctor
        public EventHub()
        {
            _repository = InMemoryRepository.GetInstance();
        }

        #endregion

        public List<string> LoadEventChatRooms()
        {
            return _eventChatRooms;
        }


        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
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

        public async Task JoinEventChatRoom(string roomName){
            if(!_eventChatRooms.Contains(roomName)){
                throw new Exception($"Room[{roomName}] not found! ");
            }
            await Groups.Add(Context.ConnectionId,roomName);
            var user = _repository.GetChatUserByConnectionId(Context.ConnectionId);
            Clients.Group(roomName).newUserJoinedEventChatRoom(roomName, user.Username);
        }

        #region  Connect Event Handler Methods
        public override Task OnDisconnected(bool stopCalled)
        {
            var user = _repository.GetChatUserByConnectionId(Context.ConnectionId);
            if (user != null)
            {
                _repository.Remove(user);
                _repository.RemoveMapping(Context.ConnectionId);
                Clients.All.leaves(user.Id, user.Username, DateTime.Now);
            }
            return base.OnDisconnected(stopCalled);
        }

        #endregion


        #region Utilities
        private void InitEventChatRooms()
        {
            _eventChatRooms.Add("1");
            _eventChatRooms.Add("2");
            _eventChatRooms.Add("3");
            _eventChatRooms.Add("4");
            _eventChatRooms.Add("5");
        }

        #endregion
    }
}
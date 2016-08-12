using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace SparxoChat.Hubs
{
    public class EventHub : Hub
    {
        #region Fields
        private readonly List<string> _eventChatRooms = new List<string>();
        private static readonly ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _clients = new ConcurrentDictionary<string, string>();

        #endregion

        public EventHub()
        {

        }

        public List<string> LoadEventChatRooms()
        {
            return _eventChatRooms;
        }


        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public void LogOn(string userName){
            string user;
            if(_clients.TryGetValue(Context.ConnectionId,out user)){
                
            }
        }

        public async Task JoinEventChatRoom(string roomName){
            if(!_eventChatRooms.Contains(roomName)){
                throw new Exception($"Room[{roomName}] not found! ");
            }
            await Groups.Add(Context.ConnectionId,roomName);
            Clients.Group(roomName).newUserJoinedEventChatRoom(roomName,Context.ConnectionId);
        }


        #region Utilities
        private void InitEventChatRooms()
        {
            _eventChatRooms.Add("1");
            _eventChatRooms.Add("2");
            _eventChatRooms.Add("3");
            _eventChatRooms.Add("4");
            _eventChatRooms.Add("5");
        }

        private string GetUser(string connectionId)
        {
            string userName;
            if (!_clients.TryGetValue(connectionId, out userName))
            {
                return connectionId;
            }
            return userName;
        }

        private string GetClient(string userName){
            string connectionId;
            if(_users.TryGetValue(userName, out connectionId)){
                return connectionId;
            }
            return null;
        }
        #endregion
    }
}
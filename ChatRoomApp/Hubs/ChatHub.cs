using ChatRoomApp.DataService;
using ChatRoomApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomApp.Hubs
{
    // this is used to create a websocket for sending and receiving messages between clients
    // the methods defined here provide functionality for the chatroom app
    public class ChatHub : Hub

    {
        // this creates in-memory database to save data inside of a dictionary
        private readonly SharedDb _shared;

        public ChatHub(SharedDb shared) => _shared = shared;
     
        public async Task JoinChat(UserConnection conn)
        {
            // this ensures that all clients see when someone has connected
            await Clients.All.SendAsync("ReceiveMessage", "admin",$"{conn.Username} has joined.");
        }

        public async Task JoinSpecificChatRoom(UserConnection conn)
        {
            // this allows the user to join a given chatroom and displays a message when they join
            // the connectionId refers to the unique connection made by the websocket between the client and the server
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);

            // this adds the connection id to each user and their information to the database
            _shared.connections[Context.ConnectionId] = conn;

            await Clients.Group(conn.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin",$"{conn.Username} has joined {conn.ChatRoom}");
        }

        public async Task SendMessage(string msg)
        {
            // this checks the connection id and the user details of the one making the request
            if(_shared.connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.ChatRoom).SendAsync("ReceiveSpecificMessage", conn.Username, msg);
            }
        }
    }
}

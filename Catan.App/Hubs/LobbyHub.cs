using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Catan.App;

namespace Catan.App.Hubs
{
    public class LobbyHub : Hub
    {
        public readonly static Lobby _lobby = new Lobby();

        public async Task SendMessage(Message message)
        {
            message.SenderType = Context.ConnectionId;
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            // In the future some sort of authentication should happen here. For now, create a generated user per connection.
            User newUser = new User()
            {
                Id = Context.ConnectionId,
                Username = Context.ConnectionId
            };
            _lobby.AddConnectedUser(Context.ConnectionId, newUser);
            await Clients.All.SendAsync("ReceiveMessage", new Message()
            {
                User = Context.ConnectionId,
                SenderType = "server",
                Content = Context.ConnectionId + " connected."
            });
            await Clients.Caller.SendAsync("ReceiveAllConnectedUsers", _lobby.ConnectedUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _lobby.RemoveConnectedUser(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveMessage", new Message()
            {
                User = Context.ConnectionId,
                SenderType = "server",
                Content = Context.ConnectionId + " disconnected."
            });
        }

        public async Task CreateGame(Game game)
        {
            game.InProgress = false;
            game.Id = _lobby.GenerateNewGameId();
            User hostUser = _lobby.FindUserByConnectionId(Context.ConnectionId);
            game.Host = hostUser;
            game.Players.Add(new Player(hostUser));
            _lobby.Games.Add(game);
            Console.WriteLine(_lobby.Games.Count);
            await Clients.All.SendAsync("GameCreated", game);
        }

        public async Task JoinGame(string gameId)
        {
            Game updatedGame = _lobby.AddUserToGame(Context.ConnectionId, gameId);
            await Clients.All.SendAsync("GameUpdated", updatedGame);
        }

        public async Task LeaveGame(string gameId)
        {
            Game updatedGame = _lobby.RemoveUserFromGame(Context.ConnectionId, gameId);
            if (updatedGame == null)
            {
                await Clients.All.SendAsync("GameRemoved", gameId);
            }
            else
            {
                await Clients.All.SendAsync("GameUpdated", updatedGame);
            }
        }
    }
}
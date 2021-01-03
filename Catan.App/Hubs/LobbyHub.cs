using System;
using System.Linq;
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
            message.Sender = Context.ConnectionId;
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
            await Clients.All.SendAsync("UserConnected", newUser);
            await Clients.Caller.SendAsync("ReceiveAllConnectedUsers", _lobby.ConnectedUsers.ToList());
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            User disconnectedUser = _lobby.FindUserByConnectionId(Context.ConnectionId);
            Game gameHostedByUser = _lobby.FindGameByHost(disconnectedUser);
            Game gameJoinedByUser = _lobby.FindGameWithUser(disconnectedUser);
            if (gameHostedByUser != null)
            {
                _lobby.RemoveGame(gameHostedByUser.Id);
                await Clients.All.SendAsync("GameRemoved", gameHostedByUser.Id);
            }
            else if (gameJoinedByUser != null)
            {
                Game updatedGame = gameJoinedByUser.HandleDisconnectedUser(disconnectedUser);
                await Clients.All.SendAsync("GameUpdated", updatedGame);
            }
            _lobby.RemoveConnectedUser(Context.ConnectionId);
            await Clients.All.SendAsync("UserDisconnected", disconnectedUser);
        }

        public async Task GetAllGames()
        {
            await Clients.Caller.SendAsync("ReceiveAllGames", _lobby.Games);
        }

        public async Task CreateGame(Game game)
        {
            game.InProgress = false;
            game.Id = _lobby.GenerateNewGameId();
            User hostUser = _lobby.FindUserByConnectionId(Context.ConnectionId);
            game.Host = hostUser;
            game.Players.Add(new Player(hostUser));
            _lobby.Games.Add(game);
            await Clients.All.SendAsync("GameCreated", game);
            await Clients.Caller.SendAsync("GameJoined", game.Id);
        }

        public async Task JoinGame(string gameId)
        {
            try
            {
                Game updatedGame = _lobby.AddUserToGame(Context.ConnectionId, gameId);
                await Clients.All.SendAsync("GameUpdated", updatedGame);
                await Clients.Caller.SendAsync("GameJoined", updatedGame.Id);
            }
            catch (Exception error)
            {
                await Clients.Caller.SendAsync("Error", error.Message);
            }
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
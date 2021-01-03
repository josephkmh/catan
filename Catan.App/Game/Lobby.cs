using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Catan.App
{
    public class Lobby
    {
        public List<Game> Games { get; set; }
        public ConcurrentDictionary<string, User> ConnectedUsers { get; set; }
        public Lobby()
        {
            this.Games = new List<Game>();
            this.ConnectedUsers = new ConcurrentDictionary<string, User>();
        }
        public bool AddConnectedUser(string connectionId, User user)
        {
            Console.WriteLine("Adding user " + user.Id);
            return this.ConnectedUsers.TryAdd(connectionId, user);
        }
        public bool RemoveConnectedUser(string connectionId)
        {
            Console.WriteLine("Removing user " + connectionId + ". Users in Lobby: " + this.ConnectedUsers.Count);
            foreach (var game in this.Games)
            {
                if (game.Host.Id == connectionId)
                {
                    this.Games.Remove(game);
                }
                else
                {
                    game.HandleDisconnectedUser(this.FindUserByConnectionId(connectionId));
                }
            }
            return this.ConnectedUsers.TryRemove(connectionId, out _);
        }

        public bool RemoveGame(string gameId)
        {
            Game gameToRemove = this.Games.Where(g => g.Id == gameId).SingleOrDefault();
            if (gameToRemove == null)
            {
                throw new Exception("Could not find game to remove (invalid game ID");
            }
            return this.Games.Remove(gameToRemove);
        }

        public Game AddUserToGame(string connectionId, string gameId)
        {
            User user = this.FindUserByConnectionId(connectionId);
            Game gameToJoin = this.Games.Where(g => g.Id == gameId).SingleOrDefault();
            if (gameToJoin == null)
            {
                throw new Exception("Could not find game to join (invalid game ID)");
            }

            if (gameToJoin.Players.Count >= gameToJoin.MaxPlayers)
            {
                throw new Exception("Game is full");
            }
            Player player = new Player(user);
            gameToJoin.Players.Add(player);
            return gameToJoin;
        }

        public Game RemoveUserFromGame(string connectionId, string gameId)
        {
            Game gameToLeave = this.Games.Where(g => g.Id == gameId).SingleOrDefault();
            if (gameToLeave == null)
            {
                throw new Exception("Could not find game to leave (invalid game ID)");
            }
            Console.WriteLine("Removing user " + connectionId + " from game " + gameToLeave.Id);
            gameToLeave.Players.RemoveAll(p => p.User.Id == connectionId);
            if (gameToLeave.Host.Id == connectionId)
            {
                this.RemoveGame(gameToLeave.Id);
                return null;
            }
            return gameToLeave;
        }

        public User FindUserByConnectionId(string connectionId)
        {
            User user = this.ConnectedUsers[connectionId];
            if (user == null)
                throw new Exception("No user found with connectionId " + connectionId);

            return user;
        }

        private string GenerateAlphanumericId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public string GenerateNewGameId()
        {
            string id;
            do
            {
                id = this.GenerateAlphanumericId();
            }
            while (this.Games.Any(g => g.Id == id));
            return id;
        }

        public Game FindGameByHost(User user)
        {
            return Games.Where(game => game.Host == user).SingleOrDefault();
        }

        public Game FindGameWithUser(User user)
        {
            return Games.SingleOrDefault(game => Enumerable.Any<Player>(game.Players, player => player.User == user));
        }
    }
}
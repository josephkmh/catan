using System;
using System.Collections.Generic;
using System.Linq;

namespace Catan.App
{
    public class Game
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool InProgress { get; set; }
        public int MaxPlayers { get; set; }
        public User Host { get; set; }
        public List<Player> Players { get; set; }
        public Game()
        {
            this.Players = new List<Player>();
        }

        public Game HandleDisconnectedUser(User user)
        {
            if (!this.InProgress)
            {
                var playerToRemove = Players.Where(player => player.User.Id == user.Id).SingleOrDefault();

                if (user != null)
                {
                    Players.Remove(playerToRemove);
                }

                return this;
            }
            else
            {
                throw new Exception("User has left a game that is in progress");
            }
        }
    }
}
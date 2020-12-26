using System.Collections.Generic;

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
            this.MaxPlayers = 4;
        }
    }
}
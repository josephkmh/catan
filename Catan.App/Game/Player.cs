namespace Catan.App
{
    public class Player
    {
        public User User { get; set; }
        public Game Game { get; set; }
        public Player(User user)
        {
            this.User = user;
        }
    }
}
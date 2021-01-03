namespace Catan.App
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public Game CurrentGame { get; set; }
    }
}
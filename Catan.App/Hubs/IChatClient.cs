using System.Threading.Tasks;

namespace otilos.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
    }
}
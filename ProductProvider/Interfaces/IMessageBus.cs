using System.Threading.Tasks;

namespace ProductProvider.Services
{
    public interface IMessageBus
    {
        Task PublishAsync(string messageType, object messageData);
    }
}

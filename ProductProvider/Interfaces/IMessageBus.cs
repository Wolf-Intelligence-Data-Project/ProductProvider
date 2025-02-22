using System.Threading.Tasks;

namespace ProductProvider.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync(string messageType, object messageData);
    }
}

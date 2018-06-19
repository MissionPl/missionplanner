using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Service.Messaging
{
    public class MessagesService : IMessagesService
    {
        public MessagesService()
        {
            Messages = new ObservableProperty<Message>(0);
        }

        public ObservableProperty<Message> Messages { get; }

        public Task AddMessageAsync(Message message)
        {
            Console.WriteLine(message.Content);
#if DEBUG
            Debug.WriteLine(message.Content);
#endif
            return Task.Factory.StartNew(() => Messages.Value = message);
        }
    }
}

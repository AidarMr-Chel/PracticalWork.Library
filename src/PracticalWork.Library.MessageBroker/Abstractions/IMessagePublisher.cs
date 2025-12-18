using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.MessageBroker.Abstractions
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message) where T : class;
    }

}

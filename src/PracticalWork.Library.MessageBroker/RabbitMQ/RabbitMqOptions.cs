using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.MessageBroker.RabbitMQ
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = default!;
        public string User { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Exchange { get; set; } = "library.events";
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Events.Readers
{
    public sealed class ReaderClosedEvent
    {
        public Guid ReaderId { get; init; }
        public DateTime ClosedAt { get; init; }
    }

}

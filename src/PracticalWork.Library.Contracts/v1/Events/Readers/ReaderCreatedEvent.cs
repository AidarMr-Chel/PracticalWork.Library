using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Events.Readers
{
    public sealed class ReaderCreatedEvent
    {
        public Guid ReaderId { get; init; }
        public string FullName { get; init; } = default!;
        public string PhoneNumber { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
    }

}

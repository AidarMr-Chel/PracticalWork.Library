using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Events.Borrows
{
    public sealed class BookReturnedEvent
    {
        public Guid BorrowId { get; init; }
        public Guid BookId { get; init; }
        public Guid ReaderId { get; init; }
        public DateTime ReturnedAt { get; init; }
    }

}

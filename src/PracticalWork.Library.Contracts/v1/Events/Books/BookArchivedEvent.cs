using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Events.Books
{
    public sealed class BookArchivedEvent
    {
        public Guid BookId { get; init; }
        public DateTime ArchivedAt { get; init; }
    }

}

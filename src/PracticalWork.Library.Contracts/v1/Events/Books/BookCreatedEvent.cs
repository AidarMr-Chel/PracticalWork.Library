using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Events.Books
{
    public sealed class BookCreatedEvent
    {
        public Guid BookId { get; init; }
        public string Title { get; init; } = default!;
        public string Category { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
    }

}

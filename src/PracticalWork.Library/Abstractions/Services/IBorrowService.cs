using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Abstractions.Services
{
    public interface IBorrowService
    {
        Task<Guid> CreateBorrow(Guid bookId, Guid readerId);
    }
}

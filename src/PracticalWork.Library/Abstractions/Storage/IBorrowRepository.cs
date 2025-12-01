using PracticalWork.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Abstractions.Storage
{
    public interface IBorrowRepository
    {
        Task<Guid> CreateBorrow(Guid bookId, Guid readerId);

    }
}

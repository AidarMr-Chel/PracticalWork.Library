using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Borrows.Response
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    public sealed record CreateBorrowResponse(Guid Id);
}

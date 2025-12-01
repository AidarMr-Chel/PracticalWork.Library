using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Readers.Request
{
    /// <summary>
    /// Запрос на продление
    /// </summary>
    /// <param name="ExpiryDate"></param>
    public sealed record ExtendReaderRequest(DateOnly ExpiryDate);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Readers.Request
{
    /// <summary>
    /// Запрос на создание читателя
    /// </summary>
    /// <param name="FullName"></param>
    /// <param name="PhoneNumber"></param>
    /// <param name="ExpiryDate"></param>
    public sealed record CreateReaderRequest(string FullName, string PhoneNumber, DateOnly ExpiryDate);
}

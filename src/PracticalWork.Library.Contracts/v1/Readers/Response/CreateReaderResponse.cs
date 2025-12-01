using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Readers.Response
{
    /// <summary>
    /// Ответ на создание читателя
    /// </summary>
    /// <param name="Id"></param>
    public sealed record CreateReaderResponse(Guid Id);
}
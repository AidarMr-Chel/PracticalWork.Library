using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Readers.Request;
using PracticalWork.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Controllers.Mappers.v1
{
    public static class ReaderExtensions
    {
        public static Reader ToReader(this CreateReaderRequest request) =>
        new()
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            ExpiryDate = request.ExpiryDate
        };
    }
}

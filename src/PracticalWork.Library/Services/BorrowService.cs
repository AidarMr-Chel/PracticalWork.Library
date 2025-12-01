using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Services
{
    public sealed class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;
        public BorrowService(IBorrowRepository borrowRepository)
        {
            _borrowRepository = borrowRepository;
        }

        public async Task<Guid> CreateBorrow(Guid bookId, Guid readerId)
        {
            try
            {
                return await _borrowRepository.CreateBorrow(bookId, readerId);
            }
            catch (Exception ex)
            {
                throw new BookServiceException("Ошибка создание записи!", ex);
            }
        }
    }
}

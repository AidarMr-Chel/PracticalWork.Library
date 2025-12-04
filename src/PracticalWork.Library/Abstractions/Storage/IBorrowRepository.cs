using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage
{
    public interface IBorrowRepository
    {
        Task<Borrow> GetActiveBorrowAsync(Guid bookId);
        Task<Guid> AddBorrowAsync(Borrow borrow);
        Task UpdateBorrowAsync(Borrow borrow);
        Task<Borrow> GetByIdAsync(Guid id);
        Task<Borrow> GetByReaderIdAsync(Guid readerId);
    }
}

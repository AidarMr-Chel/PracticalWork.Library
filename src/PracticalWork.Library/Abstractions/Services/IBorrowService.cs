using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services
{
    public interface IBorrowService
    {
        Task<Guid> CreateBorrow(Guid bookId, Guid readerId);
        Task ReturnBook(Guid bookId);
        Task<IEnumerable<Book>> GetAvailableBooksAsync(Book filter);
        Task<Borrow> GetByIdAsync(Guid id);
        Task<Borrow> GetByReaderIdAsync(Guid readerId);
        Task<Borrow> GetDetailsAsync(string idOrReader);
    }
}

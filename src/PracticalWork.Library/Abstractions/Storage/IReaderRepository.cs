using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage
{
    public interface IReaderRepository
    {
        Task<Reader> GetByIdAsync(Guid id);
        Task<Reader> GetByPhoneAsync(string phone);
        Task AddAsync(Reader reader);
        Task UpdateAsync(Reader reader);
        Task<IEnumerable<Book>> GetBooksByReaderIdAsync(Guid readerId);
        Task<Reader> GetByNameAsync(string fullName);
    }
}

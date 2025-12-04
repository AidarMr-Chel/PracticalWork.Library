using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services
{
    public interface IReaderService
    {
        Task<Guid> CreateReader(Reader reader);
        Task ExtendReader(Guid id, DateOnly newDate);
        Task CloseReader(Guid id);
        Task<IEnumerable<Book>> GetBook(Guid id);
        Task<Guid?> FindReaderIdByPhoneAsync(string phone);
        Task<Guid?> FindReaderIdByNameAsync(string fullName);
    }
}

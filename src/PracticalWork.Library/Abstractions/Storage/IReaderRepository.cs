using PracticalWork.Library.Models;


namespace PracticalWork.Library.Abstractions.Storage
{
    public interface IReaderRepository
    {
        Task<Guid> CreateReader(Reader reader);
        Task ExtendReader(Guid id, DateOnly newDate);
        Task CloseReader(Guid id);
        Task<IEnumerable<Book>> GetBook(Guid id);
    }
}

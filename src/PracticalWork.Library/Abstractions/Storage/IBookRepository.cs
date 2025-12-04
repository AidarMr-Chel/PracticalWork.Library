using PracticalWork.Library;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IBookRepository
{
    Task<Guid> AddAsync(Book book);
    Task<Book> GetByIdAsync(Guid id);
    Task UpdateAsync(Book book);
    Task<IEnumerable<Book>> FindAsync(Book filter);
}



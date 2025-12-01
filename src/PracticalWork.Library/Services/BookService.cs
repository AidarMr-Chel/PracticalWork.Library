using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Guid> CreateBook(Book book)
    {
        book.Status = BookStatus.Available;
        try
        {
            return await _bookRepository.CreateBook(book);
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка создание книги!", ex);
        }
    }

    public async Task UpdateBook(Guid id, Book book)
    {
        try
        {
            await _bookRepository.UpdateBook(id, book);
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка обновления книги!", ex);
        }
    }

    public async Task<Book> ArchivingBook(Guid id)
    {
        try
        {
            return await _bookRepository.ArchivingBook(id);
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка архивирования книги!", ex);
        }
    }

    public async Task<IEnumerable<Book>> GetBooks(Book book)
    {
        try
        {
            return await _bookRepository.GetBooks(book);
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка получения списка книг!", ex);
        }
    }
}
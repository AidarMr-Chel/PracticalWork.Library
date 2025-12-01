using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services
{
    public sealed class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;

        public ReaderService(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;
        }
        public async Task<Guid> CreateReader(Reader reader)
        {
            try
            {
                return await _readerRepository.CreateReader(reader);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ошибка создание карточки читателя!", ex);
            }
        }
        public async Task ExtendReader(Guid id, DateOnly newDate)
        {
            try
            {
                await _readerRepository.ExtendReader(id, newDate);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ошибка продления карточки!", ex);
            }
        }
        public async Task CloseReader(Guid id)
        {
            try
            {
                await _readerRepository.CloseReader(id);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ошибка закрытия карточки!", ex);
            }
        }
        public async Task<IEnumerable<Book>> GetBook(Guid id)
        {
            try
            {
                return await _readerRepository.GetBook(id);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ошибка получения книг!", ex);
            }
        }
    }
}

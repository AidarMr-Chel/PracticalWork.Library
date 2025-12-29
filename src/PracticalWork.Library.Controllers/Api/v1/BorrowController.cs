using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Api.v1
{
    /// <summary>
    /// Контроллер для управления процессами выдачи и возврата книг.
    /// Предоставляет операции создания выдачи, возврата книги,
    /// получения списка доступных книг и получения деталей выдачи.
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/library")]
    public class BorrowController : Controller
    {
        private readonly IBorrowService _borrowService;
        private readonly IMinioService _minioService;

        public BorrowController(IBorrowService borrowService, IMinioService minioService)
        {
            _borrowService = borrowService;
            _minioService = minioService;
        }

        /// <summary>
        /// Создание новой выдачи книги читателю.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <param name="readerId">Идентификатор читателя.</param>
        /// <returns>Идентификатор созданной выдачи.</returns>
        [HttpPost("borrow")]
        public async Task<IActionResult> CreateBorrow([FromQuery] Guid bookId, [FromQuery] Guid readerId)
        {
            var borrowId = await _borrowService.CreateBorrow(bookId, readerId);
            return Ok(new { BorrowId = borrowId });
        }

        /// <summary>
        /// Возврат книги в библиотеку.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <returns>Статус 204 NoContent при успешном возврате.</returns>
        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromQuery] Guid bookId)
        {
            await _borrowService.ReturnBook(bookId);
            return NoContent();
        }

        /// <summary>
        /// Получение списка доступных (не архивированных) книг.
        /// </summary>
        /// <param name="filter">Фильтр для поиска книг.</param>
        /// <returns>Список доступных книг.</returns>
        [HttpGet("books")]
        public async Task<IActionResult> GetBooks([FromQuery] Book filter)
        {
            var books = await _borrowService.GetAvailableBooksAsync(filter);
            return Ok(books);
        }

        /// <summary>
        /// Получение деталей выдачи книги по идентификатору выдачи или имени читателя.
        /// </summary>
        /// <param name="idOrReader">Идентификатор выдачи или имя читателя.</param>
        /// <returns>Детали выдачи книги, включая ссылку на обложку.</returns>
        [HttpGet("{idOrReader}/details")]
        public async Task<IActionResult> GetDetails(string idOrReader)
        {
            var borrow = await _borrowService.GetDetailsAsync(idOrReader);
            if (borrow is null)
                return NotFound();

            string coverUrl = null;

            if (!string.IsNullOrEmpty(borrow.BookId.ToString()))
            {
                coverUrl = await _minioService.GetFileUrlAsync($"covers/{borrow.BookId}/cover.png");
            }

            var response = new BorrowDetailsResponse
            {
                Id = borrow.Id,
                BookId = borrow.BookId,
                ReaderId = borrow.ReaderId,
                BorrowDate = borrow.BorrowDate,
                DueDate = borrow.DueDate,
                ReturnDate = borrow.ReturnDate,
                Status = borrow.Status.ToString(),
                CoverUrl = coverUrl
            };

            return Ok(response);
        }
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Controllers.Validations.v1;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Api.v1
{
    /// <summary>
    /// Контроллер для управления процессами выдачи и возврата книг.
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/library")]
    public class BorrowController : Controller
    {
        private readonly IBorrowService _borrowService;

        public BorrowController(IBorrowService borrowService)
        {
            _borrowService = borrowService;
        }

        /// <summary>
        /// Создание новой выдачи книги читателю.
        /// </summary>
        /// <param name="query">Идентификаторы книги и читателя.</param>
        /// <returns>Идентификатор созданной выдачи.</returns>
        [HttpPost("borrow")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBorrow([FromQuery] CreateBorrowQuery query)
        {
            var borrowId = await _borrowService.CreateBorrow(query.BookId, query.ReaderId);
            return Ok(new { BorrowId = borrowId });
        }

        /// <summary>
        /// Возврат книги в библиотеку.
        /// </summary>
        /// <param name="query">Идентификатор книги.</param>
        [HttpPost("return")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReturnBook([FromQuery] ReturnBookQuery query)
        {
            await _borrowService.ReturnBook(query.BookId);
            return NoContent();
        }

        /// <summary>
        /// Получение списка доступных (не архивированных) книг.
        /// </summary>
        [HttpGet("books")]
        public async Task<IActionResult> GetBooks([FromQuery] Book filter)
        {
            var books = await _borrowService.GetAvailableBooksAsync(filter);
            return Ok(books);
        }

        /// <summary>
        /// Получение деталей выдачи книги по идентификатору выдачи или имени читателя.
        /// </summary>
        [HttpGet("{idOrReader}/details")]
        public async Task<IActionResult> GetDetails(string idOrReader)
        {
            var response = await _borrowService.GetBorrowDetailsAsync(idOrReader);
            if (response is null)
                return NotFound();

            return Ok(response);
        }
    }
}

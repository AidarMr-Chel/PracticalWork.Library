using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Contracts.v1.Readers.Request;
using PracticalWork.Library.Contracts.v1.Readers.Response;
using PracticalWork.Library.Controllers.Mappers.v1;
using PracticalWork.Library.Services;

namespace PracticalWork.Library.Controllers.Api.v1
{
    /// <summary>
    /// Контроллер для управления читательскими билетами
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/readers")]
    public class ReadersController : Controller
    {
        private readonly IReaderService _readerService;

        public ReadersController(IReaderService readerService)
        {
            _readerService = readerService;
        }

        /// <summary>
        /// Создание читательского билета
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateReaderResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateReader([FromBody] CreateReaderRequest request)
        {
            var result = await _readerService.CreateReader(request.ToReader());

            return Content(result.ToString());
        }

        /// <summary>
        /// Продление читательского билета
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}/extend")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExtendReader(Guid id, [FromBody] ExtendReaderRequest request)
        {
            await _readerService.ExtendReader(id, request.ExpiryDate);
            return NoContent();
        }
        /// <summary>
        /// Закрытие читательского билета
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/close")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CloseReader(Guid id)
        {
            await _readerService.CloseReader(id);
            return NoContent();
        }


        /// <summary>
        /// Получение списка книг, взятых читателем
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}/books")]
        [ProducesResponseType(typeof(IEnumerable<BookDetailsResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBooks(Guid id)
        {
            var books = await _readerService.GetBook(id);
            if (books == null || !books.Any())
                return NotFound("Нет активных выдач для данного читателя.");
            return Ok(books);
        }
    }
}

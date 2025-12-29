using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Controllers.Mappers.v1;

namespace PracticalWork.Library.Controllers.Api.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Создание новой книги.
    /// </summary>
    /// <param name="request">Данные для создания книги.</param>
    /// <returns>Идентификатор созданной книги.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _bookService.CreateBook(request.ToBook());

        var response = new CreateBookResponse(id, request.Title);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Обновление данных книги по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор книги.</param>
    /// <param name="request">Обновлённые данные книги.</param>
    /// <returns>Код 204 в случае успешного обновления.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _bookService.UpdateBook(id, request.ToBook());
        return NoContent();
    }

    /// <summary>
    /// Архивирование книги по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор книги.</param>
    /// <returns>Информация об архивировании книги.</returns>
    [HttpPut("{id:guid}/archive")]
    [ProducesResponseType(typeof(ArchiveBookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveBook(Guid id)
    {
        var book = await _bookService.ArchivingBook(id);
        if (book is null)
            return NotFound();

        var response = new ArchiveBookResponse(book.Id, book.Title, DateTime.UtcNow);
        return Ok(response);
    }

    /// <summary>
    /// Получение списка книг с поддержкой фильтрации и пагинации.
    /// Данные кешируются в Redis.
    /// </summary>
    /// <param name="request">Параметры фильтрации и пагинации.</param>
    /// <returns>Список книг, удовлетворяющих фильтру.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBooks([FromQuery] BookFilterRequest request)
    {
        var books = await _bookService.GetBooks(
            request.ToBook(),
            request.PageNumber,
            request.PageSize
        );

        if (books == null || !books.Any())
            return NotFound("Книги не найдены по заданному фильтру.");

        var response = books.Select(b => b.ToDetailsResponse()).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Обновление деталей книги (описание и обложка).
    /// </summary>
    /// <param name="id">Идентификатор книги.</param>
    /// <param name="request">Описание и файл обложки.</param>
    /// <returns>Код 204 в случае успешного обновления.</returns>
    [HttpPost("{id:guid}/details")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateDetails(Guid id, [FromForm] UpdateBookDetailsRequest request)
    {
        using var stream = request.CoverFile.OpenReadStream();
        await _bookService.UpdateBookDetailsAsync(
            id,
            request.Description,
            stream,
            request.CoverFile.FileName,
            request.CoverFile.ContentType
        );

        return NoContent();
    }
}

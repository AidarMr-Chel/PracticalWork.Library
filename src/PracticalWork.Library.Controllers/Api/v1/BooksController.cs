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
public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary> Создание новой книги</summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreateBookResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateBookRequest request)
    {
        var result = await _bookService.CreateBook(request.ToBook());

        return Content(result.ToString());
    }

    /// <summary> Обновление книги по идентификатору</summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookRequest request)
    {
        await _bookService.UpdateBook(id, request.ToBook());
        return NoContent();
    }


    /// <summary> Архивирование книги по идентификатору</summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}/archive")]
    [ProducesResponseType(typeof(ArchiveBookResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ArchiveBookResponse>> ArchiveBook(Guid id)
    {
        var result = await _bookService.ArchivingBook(id);

        if (result is null)
            return NotFound($"Книга с Id {id} не найдена.");

        var response = new ArchiveBookResponse(id, result.Title, DateTime.Now);
        return Ok(response);
    }


    /// <summary>
    /// Получение списка книг
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBooks([FromQuery] BookFilterRequest request)
    {
        var books = await _bookService.GetBooks(request.ToBook());

        if (books == null || !books.Any())
            return NotFound("Книги не найдены по заданному фильтру.");

        var pagedBooks = books
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Ok(pagedBooks);
    }

}
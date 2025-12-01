using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Contracts.v1.Borrows.Response;
using PracticalWork.Library.Controllers.Mappers.v1;
using PracticalWork.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Controllers.Api.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/library")]
    public class BorrowController : Controller
    {
        private readonly IBorrowService _borrowService;
        private readonly IBookService _bookService;

        public BorrowController(IBorrowService borrowService, IBookService bookService)
        {
            _borrowService = borrowService;
            _bookService = bookService;
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateBorrowResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateBorrow(Guid bookId, Guid readerId)
        {
            var response = await _borrowService.CreateBorrow(bookId, readerId);
            return Ok(response);
        }

        [HttpGet("books")]
        [ProducesResponseType(typeof(IEnumerable<BookDetailsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBooks([FromBody] BookFilterRequest request)
        {
            var books = await _bookService.GetBooks(request.ToBook());
            return Ok(books);
        }

        [HttpPost("return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookRequest request)
        {
            await _borrowService.ReturnBook(request.BookId, request.ReaderId);
            return Ok();
        }

    }
}

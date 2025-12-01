using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Contracts.v1.Books.Request
{
    /// <summary>
    /// Запрос на фильтрацию списка книг
    /// </summary>
    /// <param name="Title">Название книги (поиск по подстроке)</param>
    /// <param name="Category">Категория книги</param>
    /// <param name="Authors">Авторы (поиск по одному из авторов)</param>
    /// <param name="Year">Год издания</param>
    /// <param name="Status">Статус книги</param>
    /// <param name="IsArchived">Флаг архивности</param>
    /// <param name="Description">Краткое описание книги</param>
    /// <param name="PageNumber">Номер страницы</param>
    /// <param name="PageSize">Размер страницы</param>
    public sealed record BookFilterRequest(
        string Title,
        BookCategory? Category,
        IReadOnlyList<string> Authors,
        int Year,
        BookStatus? Status,
        bool? IsArchived,
        string Description,
        int PageNumber = 1,
        int PageSize = 10
    ) : AbstractBook(Title, Authors , Description, Year);
}

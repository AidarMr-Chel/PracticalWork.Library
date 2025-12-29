using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Request
{
    /// <summary>
    /// Запрос на фильтрацию списка книг.
    /// Позволяет выполнять поиск по названию, авторам, категории,
    /// году издания, статусу и признаку архивности.
    /// Поддерживает пагинацию.
    /// </summary>
    /// <param name="Title">Название книги (поиск по подстроке).</param>
    /// <param name="Category">Категория книги.</param>
    /// <param name="Authors">Список авторов (поиск по совпадению с любым автором).</param>
    /// <param name="Year">Год издания книги.</param>
    /// <param name="Status">Статус книги.</param>
    /// <param name="IsArchived">Признак архивности книги.</param>
    /// <param name="Description">Краткое описание книги (поиск по подстроке).</param>
    /// <param name="PageNumber">Номер страницы (по умолчанию 1).</param>
    /// <param name="PageSize">Размер страницы (по умолчанию 10).</param>
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
    ) : AbstractBook(Title, Authors, Description, Year);
}

namespace PracticalWork.Library.Models;

/// <summary>
/// Данные, необходимые для отправки напоминания о возврате книги.
/// Объединяет информацию о выдаче, читателе и книге.
/// </summary>
/// <param name="BorrowId">Идентификатор выдачи книги.</param>
/// <param name="ReaderFullName">Полное имя читателя.</param>
/// <param name="ReaderEmail">Email читателя.</param>
/// <param name="BookTitle">Название книги.</param>
/// <param name="BookAuthors">Список авторов книги.</param>
/// <param name="DueDate">Дата, к которой необходимо вернуть книгу.</param>
/// <param name="DaysLeft">Количество дней, оставшихся до срока возврата.</param>
public sealed record ReminderData(
    Guid BorrowId,
    string ReaderFullName,
    string ReaderEmail,
    IReadOnlyList<string> BookAuthors,
    string BookTitle,
    DateOnly DueDate,
    int DaysLeft);

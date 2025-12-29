namespace PracticalWork.Library.Contracts.v1.Borrows.Response
{
    /// <summary>
    /// Ответ на операцию создания записи о выдаче книги.
    /// Содержит идентификатор созданной выдачи.
    /// </summary>
    /// <param name="Id">Идентификатор выдачи.</param>
    public sealed record CreateBorrowResponse(Guid Id);
}

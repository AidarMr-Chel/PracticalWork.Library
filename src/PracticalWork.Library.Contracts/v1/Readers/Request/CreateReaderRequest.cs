namespace PracticalWork.Library.Contracts.v1.Readers.Request
{
    /// <summary>
    /// Запрос на создание читателя.
    /// Содержит ФИО, номер телефона и срок действия читательского билета.
    /// </summary>
    /// <param name="FullName">Полное имя читателя.</param>
    /// <param name="PhoneNumber">Номер телефона читателя.</param>
    /// <param name="ExpiryDate">Дата окончания срока действия читательского билета.</param>
    public sealed record CreateReaderRequest(
        string FullName,
        string PhoneNumber,
        DateOnly ExpiryDate
    );
}

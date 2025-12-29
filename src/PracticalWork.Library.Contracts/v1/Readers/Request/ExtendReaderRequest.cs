namespace PracticalWork.Library.Contracts.v1.Readers.Request
{
    /// <summary>
    /// Запрос на продление срока действия читательского билета.
    /// </summary>
    /// <param name="ExpiryDate">Новая дата окончания срока действия читательского билета.</param>
    public sealed record ExtendReaderRequest(
        DateOnly ExpiryDate
    );
}

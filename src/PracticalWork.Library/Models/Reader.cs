namespace PracticalWork.Library.Models
{
    /// <summary>
    /// Читатель
    /// </summary>
    public sealed class Reader
    {
        /// <summary>Идентификатор читателя</summary>
        public Guid Id { get; set; }

        /// <summary>ФИО</summary>
        /// <remarks>Запись идет через пробел</remarks>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Номер телефона</summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>Дата окончания действия карточки</summary>
        public DateOnly ExpiryDate { get; set; }

        /// <summary>Активность карточки</summary>
        public bool IsActive { get; set; }
    }
}

namespace PracticalWork.Library.Models
{
    /// <summary>
    /// Модель читателя библиотеки.
    /// Содержит основную информацию о владельце читательского билета.
    /// </summary>
    public sealed class Reader
    {
        /// <summary>
        /// Уникальный идентификатор читателя.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Полное имя читателя.
        /// </summary>
        /// <remarks>Формат записи: Фамилия Имя Отчество через пробел.</remarks>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Номер телефона читателя.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Дата окончания действия читательского билета.
        /// </summary>
        public DateOnly ExpiryDate { get; set; }

        /// <summary>
        /// Признак активности читательского билета.
        /// </summary>
        public bool IsActive { get; set; }
    }
}

namespace PracticalWork.Reports.Entities
{
    /// <summary>
    /// Сущность лога активности
    /// </summary>
    public class ActivityLog
    {
        /// <summary>
        /// Уникальный идентификатор лога активности
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Тип события
        /// </summary>
        public string EventType { get; set; } = default!;

        /// <summary>
        /// Полезная нагрузка события
        /// </summary>
        public string Payload { get; set; } = default!;

        /// <summary>
        /// Дата и время создания лога
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

}

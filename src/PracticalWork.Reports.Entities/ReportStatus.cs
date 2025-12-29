namespace PracticalWork.Reports.Entities
{
    /// <summary>
    /// Статус отчёта, отражающий текущее состояние процесса его формирования.
    /// </summary>
    public enum ReportStatus
    {
        /// <summary>
        /// Отчёт находится в процессе формирования.
        /// </summary>
        InProgress = 0,

        /// <summary>
        /// Отчёт успешно сформирован и готов к использованию.
        /// </summary>
        Completed = 1,

        /// <summary>
        /// При формировании отчёта произошла ошибка.
        /// </summary>
        Error = 2
    }
}

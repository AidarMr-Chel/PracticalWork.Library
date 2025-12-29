namespace PracticalWork.Reports.Entities.DTO;

/// <summary>
/// DTO для представления постраничного результата
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Элементы на текущей странице
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// Общее количество элементов
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Размер страницы
    /// </summary>
    public int PageSize { get; set; }
}

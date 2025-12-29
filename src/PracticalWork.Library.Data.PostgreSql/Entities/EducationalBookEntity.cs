namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Сущность, представляющая учебное пособие.
/// </summary>
public sealed class EducationalBookEntity : AbstractBookEntity
{
    /// <summary>
    /// Учебная область или предмет, к которому относится пособие
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Учебный уровень или класс, для которого предназначено пособие
    /// </summary>
    public string GradeLevel { get; set; }
}

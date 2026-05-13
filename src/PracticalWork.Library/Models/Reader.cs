namespace PracticalWork.Library.Models
{
    /// <summary>
    /// Модель читателя библиотеки.
    /// </summary>
    public sealed class Reader
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly ExpiryDate { get; set; }
        public bool IsActive { get; set; }

        public string Email { get; set; }
    }
}
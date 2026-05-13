namespace PracticalWork.Library.Abstractions.Services;

public class EmailSendResult
{
    public bool IsSuccess { get; private set; }
    public string Error { get; private set; }

    public static EmailSendResult Success() => new() { IsSuccess = true };

    public static EmailSendResult Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
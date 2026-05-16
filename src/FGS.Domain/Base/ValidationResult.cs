namespace FGS.Domain.Base;

public sealed class ValidationResult
{
    public bool IsSuccess { get; }
   
    public Exception Exception { get; }
    
    private ValidationResult(bool isSuccess, Exception exception)
    {
        IsSuccess = isSuccess;
        Exception = exception;
    }
    
    public static ValidationResult Success => new(true, null!);
    public static ValidationResult Failure(Exception ex) => new(false, ex);
}
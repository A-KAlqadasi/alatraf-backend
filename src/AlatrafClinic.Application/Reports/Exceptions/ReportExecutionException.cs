namespace AlatrafClinic.Application.Reports.Exceptions;

public class ReportExecutionException : Exception
{
    public ReportExecutionException(string message) : base(message) { }
    public ReportExecutionException(string message, Exception inner) : base(message, inner) { }
}

public class ReportLimitExceededException : Exception
{
    public ReportLimitExceededException(string message) : base(message) { }
}
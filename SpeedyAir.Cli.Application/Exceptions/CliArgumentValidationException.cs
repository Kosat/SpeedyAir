namespace SpeedyAir.Exceptions;

internal class CliArgumentValidationException : Exception
{
    public CliArgumentValidationException(string? message) : base(message)
    {
    }
}

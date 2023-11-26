namespace SpeedyAir.Exceptions;

internal class ApplicationLogicException : Exception
{
    public Type CommandType { get; }

    public ApplicationLogicException(Type commandType, string message, Exception? innerException) : base(message, innerException)
        => CommandType = commandType;
}

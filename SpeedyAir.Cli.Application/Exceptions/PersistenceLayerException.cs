namespace SpeedyAir.Exceptions;

internal class PersistenceLayerException : Exception
{
    public PersistenceLayerException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}

namespace CS2Retake.Allocators.Exceptions;

public class AllocatorException : Exception
{
    public AllocatorException()
    {
    }

    public AllocatorException(string message) : base(message)
    {
    }

    public AllocatorException(string message, Exception inner) : base(message, inner)
    {
    }
}
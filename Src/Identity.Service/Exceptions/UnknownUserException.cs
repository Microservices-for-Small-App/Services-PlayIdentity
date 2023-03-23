namespace Identity.Service.Exceptions;

[Serializable]
internal class UnknownUserException : Exception
{
    public UnknownUserException(Guid userId) : base($"Unknown user '{userId}'")
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}


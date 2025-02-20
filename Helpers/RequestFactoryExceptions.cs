namespace WebAPITemplate.Helpers
{
    public class RequestFactoryExceptions
    {
    }
    public class SessionExpiredException : Exception
    {
        public SessionExpiredException(string message) : base(message)
        {
        }
    }
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message) : base(message)
        {
        }
    }
    public class SubscriptionExpiredException : Exception
    {
        public SubscriptionExpiredException(string message) : base(message)
        {
        }
    }
    public class InternalServerException : Exception
    {
        public InternalServerException(string message) : base(message)
        {
        }
    }
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}

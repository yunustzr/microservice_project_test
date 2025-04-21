using System.Security.Authentication;


namespace AuthenticationApi.Domain.Exceptions
{
    public class InvalidCredentialsException : AuthenticationException
    {
        public int FailedLoginAttempts { get; }
        public int RemainingAttempts { get; }
        public DateTime? LockoutEnd { get; }

        public InvalidCredentialsException(string message,
            int failedLoginAttempts,
            int maxAttempts,
            DateTime? lockoutEnd = null)
            : base(message)
        {
            FailedLoginAttempts = failedLoginAttempts;
            RemainingAttempts   = Math.Max(0, maxAttempts - failedLoginAttempts);
            LockoutEnd          = lockoutEnd;
        }
    }
}

using System;

namespace ClipVault.Exceptions
{
    public class RegistrationFailedException : Exception
    {
        public RegistrationFailedException(string message) : base(message)
        {
        }

        public RegistrationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

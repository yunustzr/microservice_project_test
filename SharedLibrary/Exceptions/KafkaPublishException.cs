namespace SharedLibrary.Exceptions;

public class KafkaPublishException : Exception
{
    public KafkaPublishException(string message) : base(message) { }

        public KafkaPublishException(string message, Exception innerException)
            : base(message, innerException) { }
}

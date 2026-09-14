namespace CustomerSupport.Application.Exceptions
{
    public sealed class ApplicationValidationException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public ApplicationValidationException(IEnumerable<string> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors.ToArray();
        }
    }
}

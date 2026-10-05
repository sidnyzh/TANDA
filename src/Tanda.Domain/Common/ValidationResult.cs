namespace Tanda.Domain.Common
{
    public sealed record ValidationResult(bool IsValid, string? Reason = null)
    {
        public static ValidationResult Valid() => new(true);
        public static ValidationResult Invalid(string reason) => new(false, reason);
    }
}

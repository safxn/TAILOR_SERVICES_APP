using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Common;

public sealed class RequiredFieldAttribute : RequiredAttribute
{
    public RequiredFieldAttribute() => ErrorMessage = "{0} is required.";
}

public sealed class LengthAttribute : StringLengthAttribute   // {0}=name, {1}=max, {2}=min
{
    public LengthAttribute(int max) : base(max) =>
        ErrorMessage = "{0} must be {1} characters or fewer.";
}

public sealed class EmailFieldAttribute : RegularExpressionAttribute
{
    public EmailFieldAttribute() : base(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") =>
        ErrorMessage = "Enter a valid email address.";
}

public sealed class PhoneFieldAttribute : RegularExpressionAttribute
{
    public PhoneFieldAttribute() : base(@"^\+?[0-9\s\-]{7,15}$") =>
        ErrorMessage = "Enter a valid phone number, e.g. +968 9123 4567.";
}

public sealed class StrongPasswordAttribute : RegularExpressionAttribute
{
    public StrongPasswordAttribute() : base(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$") =>
        ErrorMessage = "Password must be at least 8 characters with an uppercase letter, a lowercase letter, and a number.";
}
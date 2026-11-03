using Microsoft.AspNetCore.Identity;

namespace DomainLayer.Extensions;

internal sealed class AppIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length) =>
        Error(nameof(PasswordTooShort), $"Password must be at least {length} characters.");

    public override IdentityError PasswordRequiresUpper() =>
        Error(nameof(PasswordRequiresUpper), "Password must include an uppercase letter (A-Z).");

    public override IdentityError PasswordRequiresLower() =>
        Error(nameof(PasswordRequiresLower), "Password must include a lowercase letter (a-z).");

    public override IdentityError PasswordRequiresDigit() =>
        Error(nameof(PasswordRequiresDigit), "Password must include a number (0-9).");

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        Error(nameof(PasswordRequiresNonAlphanumeric), "Password must include a symbol, such as ! or #.");

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        Error(nameof(PasswordRequiresUniqueChars), $"Password must use at least {uniqueChars} different characters.");

    public override IdentityError DuplicateEmail(string email) =>
        Error(nameof(DuplicateEmail), "This email is already registered.");

    public override IdentityError DuplicateUserName(string userName) =>
        Error(nameof(DuplicateUserName), "This email is already registered.");

    public override IdentityError InvalidEmail(string? email) =>
        Error(nameof(InvalidEmail), "Enter a valid email address.");

    private static IdentityError Error(string code, string description) =>
        new() { Code = code, Description = description };
}
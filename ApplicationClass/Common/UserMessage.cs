namespace DomainLayer.Common;

public static class UserMessage
{
    public const string NotFound = "User not found.";
    public const string CannotDeleteSelf = "You cannot delete your own account.";
    public const string CannotDeleteLastAdmin = "You cannot delete the last admin.";
    public const string HasRelatedRecords = "This user has related records (customers, orders, etc.) and cannot be deleted.";
    public const string Created = "User created.";
    public const string Updated = "User updated.";
    public const string Deleted = "User deleted.";
}
using DomainLayer.Common;
using DomainLayer.Dtos.Users;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace DomainLayer;

internal sealed class UserRepository(
    UserManager<ApplicationUser> userManager,
    ILogger<UserRepository> logger) : IUserRepository                           
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<UserRepository> _logger = logger;


    //// Repository
    public async Task<DataSourceResult> GetGridAsync(DataSourceRequest request, CancellationToken ct)
        => await _userManager.Users
            .AsNoTracking()
            .Select(x => new UserGridDto { Id = x.Id, FullName = x.FullName!, Email = x.Email!, PhoneNumber = x.PhoneNumber })
            .ToDataSourceResultAsync(request);

    public async Task<OperationResult<UserDetailDto>> GetByIdAsync(string id, CancellationToken ct)
    {
        var user = await _userManager.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        if (user is null) return OperationResult<UserDetailDto>.Fail(UserMessage.NotFound);

        return OperationResult<UserDetailDto>.Ok(new UserDetailDto
        {
            Id = user.Id,
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber
        });
    }

    public async Task<OperationResult> CreateAsync(UserCreateDto dto, CancellationToken ct)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            return result.Succeeded
                ? OperationResult.Ok()
                : OperationResult.Fail([.. result.Errors.Select(e => e.Description)]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create user failed");
            return OperationResult.Fail(UserMessage.Created);
        }
    }

    public async Task<OperationResult> UpdateAsync(UserUpdateDto dto, CancellationToken ct)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user is null) return OperationResult.Fail(UserMessage.NotFound);

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email;        
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return OperationResult.Fail([.. result.Errors.Select(e => e.Description)]);

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var reset = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                if (!reset.Succeeded)
                    return OperationResult.Fail([.. reset.Errors.Select(e => e.Description)]);
            }

            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update user failed");
            return OperationResult.Fail(UserMessage.Updated);
        }
    }

    public async Task<OperationResult> DeleteAsync(string id, string currentUserId, CancellationToken ct)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return OperationResult.Fail(UserMessage.NotFound);

            if (user.Id == currentUserId)
                return OperationResult.Fail(UserMessage.CannotDeleteSelf);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                    return OperationResult.Fail(UserMessage.CannotDeleteLastAdmin);
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded
                ? OperationResult.Ok()
                : OperationResult.Fail([.. result.Errors.Select(e => e.Description)]);
        }
        catch (DbUpdateException ex)
        {
            // Restrict FKs (e.g. Customer.CreatedById) block deleting users who own records
            _logger.LogWarning(ex, "Delete user blocked by related records");
            return OperationResult.Fail("This user has related records (customers, orders, etc.) and cannot be deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete user failed");
            return OperationResult.Fail(UserMessage.Deleted);
        }
    }
}
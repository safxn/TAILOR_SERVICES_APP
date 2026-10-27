using DomainLayer.Common;
using DomainLayer.Dtos.Users;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer;


public interface IUserRepository
{
Task<DataSourceResult> GetGridAsync(DataSourceRequest request, CancellationToken ct);

Task<OperationResult<UserDetailDto>> GetByIdAsync(string id, CancellationToken ct);

Task<OperationResult> CreateAsync(UserCreateDto dto, CancellationToken ct);

Task<OperationResult> UpdateAsync(UserUpdateDto dto, CancellationToken ct);

Task<OperationResult> DeleteAsync(string id, string currentUserId, CancellationToken ct);

}
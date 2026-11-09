using DomainLayer.Common;
using DomainLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer;

public interface ICustomerRepository
{
    Task<PagedResult<CustomerGridDto>> GetGridAsync(GridRequest r, CancellationToken ct);

    Task<OperationResult<CustomerDetailDto>> GetByIdAsync(string id, CancellationToken ct);

    Task<OperationResult> CreateAsync(CustomerSaveDto dto, string userId, CancellationToken ct);

    Task<OperationResult> UpdateAsync(string id, CustomerSaveDto dto, CancellationToken ct);

    Task<OperationResult> DeleteAsync(string id, CancellationToken ct);
}

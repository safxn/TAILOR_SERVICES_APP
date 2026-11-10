using DomainLayer.Common;
using DomainLayer.Dtos;
using DomainLayer.Models;
using InfrastructureLayer;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace DomainLayer;

internal sealed class CustomerRepository(AppDbContext db, 
    ILogger<CustomerRepository> logger) : ICustomerRepository
{
    public async Task<DataSourceResult> GetGridAsync(DataSourceRequest request, CancellationToken ct)
    => await db.Customers
        .AsNoTracking()
        .Select(x => new CustomerGridDto { Id = x.Id, Name = x.Name, Phone = x.Phone, Address = x.Address, CreatedAt = x.CreatedAt })
        .ToDataSourceResultAsync(request, ct);



    public async Task<OperationResult<CustomerDetailDto>> GetByIdAsync(string id, CancellationToken ct)
    {
        var c = await db.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        return c is null
            ? OperationResult<CustomerDetailDto>.Fail("Customer not found.")
            : OperationResult<CustomerDetailDto>.Ok(new(c.Id, c.Name, c.Phone, c.Address, c.Notes));
    }

    public async Task<OperationResult> CreateAsync(CustomerSaveDto dto, string userId, CancellationToken ct)
    {
        try
        {
            db.Customers.Add(Customer.Create(dto.Name, dto.Phone!, dto.Address!, dto.Notes!, userId));
            await db.SaveChangesAsync(ct);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create customer failed");
            return OperationResult.Fail("Could not create the customer.");
        }
    }

    public async Task<OperationResult> UpdateAsync(string id, CustomerSaveDto dto, CancellationToken ct)
    {
        try
        {
            var c = await db.Customers.SingleOrDefaultAsync(x => x.Id == id, ct);
            if (c is null) return OperationResult.Fail("Customer not found.");
            c.Update(dto.Name, dto.Phone, dto.Address, dto.Notes);
            await db.SaveChangesAsync(ct);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update customer failed");
            return OperationResult.Fail("Could not update the customer.");
        }
    }

    public async Task<OperationResult> DeleteAsync(string id, CancellationToken ct)
    {
        try
        {
            var c = await db.Customers.SingleOrDefaultAsync(x => x.Id == id, ct);
            if (c is null) return OperationResult.Fail("Customer not found.");
            db.Customers.Remove(c);
            await db.SaveChangesAsync(ct);
            return OperationResult.Ok();
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Delete customer blocked");
            return OperationResult.Fail("This customer has measurements or orders and cannot be deleted.");
        }
    }
}


using InfrastructureLayer.Seed;
using Microsoft.AspNetCore.Authorization;
using DomainLayer.Extensions;
using TAILOR_SERVICES_APP.Authorization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddKendo();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageOrders", policy =>
        policy.Requirements.Add(new PermissionRequirement("ManageOrders")));

    options.AddPolicy("ManageCustomers", policy =>
        policy.Requirements.Add(new PermissionRequirement("ManageCustomers")));
});
builder.Services.AddDomainLayer(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();   
app.MapRazorPages();

app.Run();
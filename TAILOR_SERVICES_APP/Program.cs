using DomainLayer.Extensions;
using InfrastructureLayer.Seed;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Json;
using TAILOR_SERVICES_APP.Authorization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddControllers()
      .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddCors(o => o.AddPolicy("angular", p =>
    p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageOrders", policy =>
        policy.Requirements.Add(new PermissionRequirement("ManageOrders")));

    options.AddPolicy("ManageCustomers", policy =>
        policy.Requirements.Add(new PermissionRequirement("ManageCustomers")));
});
builder.Services.AddDomainLayer(builder.Configuration);

builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

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
app.UseCors("angular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();   
app.MapRazorPages();

app.Run();
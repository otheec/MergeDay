using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Features.Fakturoid;
using MergeDay.Api.Features.Toggl;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddDbContext<MergeDayDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MergeDayDb")));
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<MergeDayDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthorization();
builder.Services.AddToggl(
    builder.Configuration["Toggl:ApiToken"],
    builder.Configuration["Toggl:BaseUrl"]
);
builder.Services.AddFakturoid(builder.Configuration["Fakturoid:BaseUrl"]);

var app = builder.Build();

app.MapEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();

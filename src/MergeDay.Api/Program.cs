using MergeDay.Api.Endpoints;
using MergeDay.Api.Features.Toggl;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddDbContext<MergeDayDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MergeDayDb")));
builder.Services.AddToggl();

var app = builder.Build();

app.MapEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

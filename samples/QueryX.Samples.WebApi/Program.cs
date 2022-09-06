using Microsoft.EntityFrameworkCore;
using QueryX;
using QueryX.Samples.WebApi;
using QueryX.Samples.WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SampleContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("QueryXWebApi"))
);

builder.Services.AddHostedService<InitDbContext>();

builder.Services.AddQueryX(o =>
{
    o.SetDateTimeConverter((dateTime) => dateTime.ToUniversalTime());
    o.SetDateTimeOffsetConverter((dateTime) => dateTime.ToUniversalTime());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

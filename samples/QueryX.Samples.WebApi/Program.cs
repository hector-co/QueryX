using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX;
using QueryX.Samples.WebApi.DataAccess.EF;
using QueryX.Samples.WebApi.DataAccess.EF.Cards;
using QueryX.Samples.WebApi.Domain.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddControllersAsServices();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DescribeAllParametersInCamelCase();
});

builder.Services.AddDbContext<WorkboardContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("QueryXWebApi"))
);

builder.Services.AddMediatR(typeof(WorkboardContext));

builder.Services.AddHostedService<InitData>();

builder.Services.AddQueryX(o =>
{
    o.SetDateTimeConverter((dateTime) => dateTime.ToUniversalTime());
    o.SetDateTimeOffsetConverter((dateTime) => dateTime.ToUniversalTime());
});

builder.Services.AddScoped<ICardService, CardService>();

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

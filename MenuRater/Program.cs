using MenuRater;
using MenuRater.Data;
using MenuRater.Interfaces;
using MenuRater.Services;
using MenuRater.Services.Http;
using MenuRater.Services.RabbitMq;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("MenuRater"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMenuRateService, MenuRateService>();
builder.Services.AddHttpClient<IHttpService, HttpService>();
builder.Services.AddSingleton<IPublisherServiceFactory, PublisherServiceFactory>();
builder.Services.AddTransient<RmqPublisherService>();
builder.Services.AddTransient<HttpPublisherService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

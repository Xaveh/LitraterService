using Litrater.Application;
using Litrater.Application.Books.Queries.GetBookById;
using Litrater.Infrastructure;
using Litrater.Presentation.Books.GetBookById;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

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

// Register minimal API endpoints
GetBookByIdEndpoint.MapGetBookByIdEndpoint(app);

app.Run();
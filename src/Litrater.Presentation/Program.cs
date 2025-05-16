using Litrater.Application.Books.Queries.GetBookById;
using Litrater.Infrastructure;
using Litrater.Presentation.Books.GetBookById;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Register application services
builder.Services.AddScoped<GetBookByIdQueryHandler>();

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
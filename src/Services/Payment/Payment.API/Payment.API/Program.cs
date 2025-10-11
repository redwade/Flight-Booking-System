using BuildingBlocks.MassTransit.Configuration;
using Payment.Application;
using Payment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application and Infrastructure layers
builder.Services.AddPaymentApplication();
builder.Services.AddPaymentInfrastructure(builder.Configuration);

// Add MassTransit with RabbitMQ
builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

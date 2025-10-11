using BuildingBlocks.MassTransit.Configuration;
using Notification.Application;
using Notification.Application.Consumers;
using Notification.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application and Infrastructure layers
builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure();

// Add MassTransit with RabbitMQ and Consumers
builder.Services.AddMassTransitWithRabbitMq(builder.Configuration, cfg =>
{
    cfg.AddConsumer<BookingCreatedConsumer>();
    cfg.AddConsumer<PaymentProcessedConsumer>();
    cfg.AddConsumer<SendNotificationConsumer>();
});

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

using Amazon.SQS;
using TurnosApi.Worker;

var builder = Host.CreateApplicationBuilder(args);

// --- Configuración ---
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// --- AWS SQS Client ---
builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var config = new AmazonSQSConfig
    {
        ServiceURL = configuration["Sqs:ServiceUrl"] ?? "http://localhost:4566"
    };
    return new AmazonSQSClient("test", "test", config);
});

// --- Hosted Service (Consumer) ---
builder.Services.AddHostedService<SqsConsumerService>();

var host = builder.Build();
host.Run();

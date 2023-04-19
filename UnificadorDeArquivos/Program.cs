using Serilog;
using UnificadorDeArquivos;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File($"logs\\log_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.txt")
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddSerilog();
    })
    .Build();

await host.RunAsync();

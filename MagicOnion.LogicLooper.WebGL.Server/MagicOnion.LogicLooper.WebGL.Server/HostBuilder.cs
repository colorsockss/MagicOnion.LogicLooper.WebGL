using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using ZLogger;

namespace MagicOnion.LogicLooper.WebGL.Server;

public static class HostBuilder
{
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

        var configureWebHostDefaults = Host.CreateDefaultBuilder(args)
                                           .ConfigureServices(services =>
                                           {
                                               services.Configure<HostOptions>(options =>
                                               {
                                                   options.ShutdownTimeout = TimeSpan.FromSeconds(5);
                                               });
                                           })
                                           .ConfigureWebHostDefaults(webBuilder =>
                                           {
                                               webBuilder.UseStartup<Startup>();
                                               webBuilder.ConfigureLogging(builder =>
                                               {
                                                   builder.ClearProviders().SetMinimumLevel(LogLevel.Trace).AddZLoggerConsole();
                                               });
                                           });

        return configureWebHostDefaults;
    }
}
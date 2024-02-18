using Cysharp.Threading;

namespace MagicOnion.LogicLooper.WebGL.Server;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Register a LooperPool to the service container.
        services.AddSingleton<ILogicLooperPool>(_ => new LogicLooperPool(10, 1, RoundRobinLogicLooperPoolBalancer.Instance));
        services.AddHostedService<LoopHostedService>();

        services.AddGrpc();
        services.AddMagicOnion();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                // WARN: Do not apply following policies to your production.
                //       If not configured carefully, it may cause security problems.
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();

                // NOTE: "grpc-status" and "grpc-message" headers are required by gRPC. so, we need expose these headers to the client.
                policy.WithExposedHeaders("grpc-status", "grpc-message");
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        // NOTE: Enables static file serving only for Unity WebGL demo app.
        // app.UseDefaultFiles();
        // app.UseStaticFiles(new StaticFileOptions()
        // {
        //     ServeUnknownFileTypes = true,
        //     DefaultContentType = "application/octet-stream",
        //     OnPrepareResponse = (ctx) =>
        //     {
        //         if (ctx.File.Name.EndsWith(".br"))
        //         {
        //             ctx.Context.Response.Headers.ContentEncoding = "br";
        //         }
        //         if (ctx.File.Name.Contains(".wasm")) ctx.Context.Response.Headers.ContentType = "application/wasm";
        //         if (ctx.File.Name.Contains(".js")) ctx.Context.Response.Headers.ContentType = "application/javascript";
        //     }
        // });

        
        // Configure the HTTP request pipeline.
        // Enables CORS, WebSocket, GrpcWebSocketRequestRoutingEnabler
        // NOTE: These need to be called before `UseRouting`.  
        app.UseCors();
        app.UseWebSockets();
        app.UseGrpcWebSocketRequestRoutingEnabler();

        app.UseRouting();

        // NOTE: `UseGrpcWebSocketBridge` must be called after calling `UseRouting`.
        app.UseGrpcWebSocketBridge();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMagicOnionService();
            // "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
            endpoints.MapGet("/", () => "Get");
        });
    }
}
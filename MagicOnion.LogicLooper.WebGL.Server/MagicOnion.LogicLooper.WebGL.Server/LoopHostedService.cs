using Cysharp.Threading;

namespace MagicOnion.LogicLooper.WebGL.Server;

public class LoopHostedService(ILogicLooperPool looperPool, ILogger<LoopHostedService> logger) : IHostedService
{
    private readonly ILogicLooperPool _looperPool = looperPool ?? throw new ArgumentNullException(nameof(looperPool));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Example: Register update action immediately.
        _ = _looperPool.RegisterActionAsync((in LogicLooperActionContext ctx) =>
        {
            // _logger.LogInformation("LoopHostedService is running");
            if (ctx.CancellationToken.IsCancellationRequested)
            {
                // If LooperPool begins shutting down, IsCancellationRequested will be `true`.
                _logger.LogInformation("LoopHostedService will be shutdown soon. The registered action is shutting down gracefully.");
                return false;
            }

            return true;
        });

        // Example: Create a new world of life-game and register it into the loop.
        //   - See also: LoopHostingApp/Pages/Index.cshtml.cs
        // LifeGameLoop.CreateNew(_looperPool, _logger);

        _logger.LogInformation("LoopHostedService is started. (Loopers={LoopersCount}; TargetFrameRate={TargetFrameRate}fps)", _looperPool.Loopers.Count, _looperPool.Loopers[0].TargetFrameRate);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        _logger.LogInformation("LoopHostedService is shutting down. Waiting for loops");

        // Shutdown gracefully the LooperPool after 5 seconds.
        await _looperPool.ShutdownAsync(TimeSpan.FromSeconds(5));

        // Count remained actions in the LooperPool.
        var remainedActions = _looperPool.Loopers.Sum(x => x.ApproximatelyRunningActions);
        _logger.LogInformation("{RemainedActions} actions are remained in loop", remainedActions);
    }
}
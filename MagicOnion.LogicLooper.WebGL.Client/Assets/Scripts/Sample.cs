using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.LogicLooper.WebGL.Shared;
using UnityEngine;

public class Sample : MonoBehaviour
{
    private async void Start()
    {
        // Connect to the server using gRPC channel.
        var channel = GrpcChannel.ForAddress("http://localhost:5063", new GrpcChannelOptions
        {
            HttpHandler = new GrpcWebSocketBridge.Client.GrpcWebSocketBridgeHandler()
        });

        // NOTE: If your project targets non-.NET Standard 2.1, use `Grpc.Core.Channel` class instead.
        // var channel = new Channel("localhost", 5001, new SslCredentials());

        // Create a proxy to call the server transparently.
        var client = MagicOnionClient.Create<IMyFirstService>(channel);

        // Call the server-side method using the proxy.
        var result = await client.SumAsync(123, 456);
        
        Debug.Log(result); // 579
    }
}
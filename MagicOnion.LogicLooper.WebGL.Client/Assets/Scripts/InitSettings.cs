using JetBrains.Annotations;
using MagicOnion.Client;
using MagicOnion.LogicLooper.WebGL.Shared;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;
#endif

[UsedImplicitly]
[MagicOnionClientGeneration(typeof(IMyFirstService), Serializer = GenerateSerializerType.MemoryPack)]
internal partial class MagicOnionGeneratedClientInitializer { }

[UsedImplicitly]
internal class InitialSettings
{
    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterResolvers()
    {
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSynchronizationContext()
    {
        System.Threading.SynchronizationContext.SetSynchronizationContext(null);
    }
#endif
}
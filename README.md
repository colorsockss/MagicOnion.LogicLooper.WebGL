# MagicOnion.LogicLooper.WebGL
Cysharp server framework boilerplate example.


# MagicOnion with Unity Installation

## Prepare

먼저 하지 않으면 중간에 컴파일 에러 발생시 진행 안되는 것들

1. Nuget for Unity 설치
    https://github.com/GlitchEnzo/NuGetForUnity
2. CsprojModifier 설치
    https://github.com/Cysharp/CsprojModifier

## 클라이언트

### nuget으로 설치 해야 하는 것들
1. MicroSoft.CodeAnalysis
    MagicOnion, MemoryPack 여기서 SourceGenerator 사용하는데 여기서 필요
2. MicroSoft.CodeAnalysis.CSharp
    MicroSoft.CodeAnalysis 설치하면 의존성때문에 같이 설치 되는데 에러나는 것들 지우다 보면 같이 지워져서 따로 받아야 함.
3. Microsoft.Extensions.Logging.Abstractions.8.0.0
4. ZLogger

### unitypackage 파일로 설치 한 것들
1. MagicOnion
2. MemoryPack
    MagicOnion serializer로 MemoryPack 사용하기 위한 코드
```csharp 
    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterResolvers()
    {
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;
    }
```

3. GrpcWebSocketBridge

유니티에서는 나지 않지만 IDE에서 에러나는 것들이 있어서 패키지 형태로 구성되어 있는 것들은 다 패키지 폴더로 이동시키기

### 기타 설치 패키지들

- ZLogger.Unity
1. Install ZLogger.Unity package via git url.
    Add https://github.com/Cysharp/ZLogger.git?path=src/ZLogger.Unity/Assets/ZLogger.Unity to Package Manager
    git이 설치 안된 곳에서는 소스코드 다운 받아서 해당 경로에 있는 파일 직접 유니티 Package 폴더로 복사
	
### WebGL 추가 작업

1. GrpcWebSocketBridge webgl custom 
    release 페이지에 가서 Grpc.Net.Client-ModifiedForWebGL.x.x.x.zip 파일 받아서 안에 있는 DLL들 교체
2. Disable SynchronizationContext (WebGL)
    https://github.com/Cysharp/GrpcWebSocketBridge?tab=readme-ov-file#disable-synchronizationcontext-webgl

    초기화 클래스 생성후 아래 코드 삽입
```csharp
    #if UNITY_WEBGL && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSynchronizationContext()
    {
        SynchronizationContext.SetSynchronizationContext(null);
    }
    #endif
```

## ShareProject 셋팅
- Class Library 프로젝트로 프로젝트 추가
    1. 프로젝트 구조는 여기 참고 https://github.com/Cysharp/MagicOnion/tree/main/samples/ChatApp
    2. WebGL 은 여기 참고 https://github.com/Cysharp/GrpcWebSocketBridge/tree/main/samples
    3. Unity Package 폴더 밑으로 적당한 폴더 만들어서 이동시키기

- package.json 만들어서 패키지 형태로 구성
- 유니티에서 asmdef 파일 추가
- Directory.Build.props 파일 추가(빌드시 만들어지는 폴더 유니티에 숨기기위해)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!--
      prior to .NET 8
      <BaseIntermediateOutputPath>.artifacts\obj\</BaseIntermediateOutputPath>
		  <BaseOutputPath>.artifacts\bin\</BaseOutputPath>
    -->

    <!-- after .NET 8: https://learn.microsoft.com/en-us/dotnet/core/sdk/artifacts-output -->
    <!-- Unity ignores . prefix folder -->
    <ArtifactsPath>$(MSBuildThisFileDirectory).artifacts</ArtifactsPath>
  </PropertyGroup>
</Project>
```
- 유니티 프로젝트에서 위에서 만든 패키지 디펜던시로 추가
```
{
  "dependencies": {
    "com.your.package.shared.unity": "file:../../YourPackageSharedUnity",
  }
}
```

- 문서에는 MemoryPack이 아니고 MessagePack을 사용하는 방법으로 Code Generate 섹션이 있는데
  MagicOnion, MemoryPack 을 사용해서 SourceGenerator 가 코드를 생성해주기 때문에 스킵

- 프로젝트 파일 참고
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
      <TargetFrameworks>netstandard2.1;net8.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <None Remove="**\package.json" />
    <None Remove="**\*.asmdef" />
    <None Remove="**\*.meta" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="MagicOnion.Abstractions" Version="6.0.0" />
    <PackageReference Include="MemoryPack.Generator" Version="1.10.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="MemoryPack.UnityShims" Version="1.10.0" />
  </ItemGroup>

</Project>
```
- 
  
## 서버 프로젝트

- ShareProject 이것만 ProjectRefrence로 추가하면 나머지는 그냥 시키는대로만 하면 잘 돼서 설명서 잘 따라하면 됨

이상 1주일간 삽질기 마무리....
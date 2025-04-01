
using GlucoseTray.Read.Dexcom;
using NSubstitute;
using System.Text.Json;

namespace GlucoseTray.Tests.DSL;

internal class BehaviorDriver(DslProvider provider, GlucoseReading glucoseResult, DexcomResult dexcomResult)
{
    public BehaviorDriver RefreshingIcon()
    {
        provider.Reader.GetLatestGlucoseAsync().Returns(glucoseResult);
        provider.Runner.Process().Wait();
        return this;
    }

    public BehaviorDriver GettingLatestDexcomReading()
    {
        provider.ExternalCommunicationAdapter.PostApiResponseAsync(Arg.Any<string>(), Arg.Is<string>(x => x.Contains("bob"))).Returns("1account");
        provider.ExternalCommunicationAdapter.PostApiResponseAsync(Arg.Any<string>(), Arg.Is<string>(x => x.Contains("1account"))).Returns("1session");
        var data = JsonSerializer.Serialize(new List<DexcomResult> { dexcomResult });
        provider.ExternalCommunicationAdapter.PostApiResponseAsync(Arg.Any<string>(), Arg.Is<string>(x => x.Contains("1session"))).Returns(data);
        provider.Runner.Process().Wait();
        return this;
    }

    public AssertionDriver Then => new(provider);
}

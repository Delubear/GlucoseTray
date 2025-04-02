using GlucoseTray.Read.Dexcom;
using GlucoseTray.Read.Nightscout;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text.Json;

namespace GlucoseTray.Tests.DSL.Read;

internal class ReadBehaviorDriver(ReadProvider provider, DexcomResult dexcomResult, NightScoutResult nightscoutResult)
{
    public ReadBehaviorDriver GettingLatestDexcomReading()
    {
        provider.ExternalCommunicationAdapter.PostApiResponseAsync(Arg.Any<string>(), Arg.Is<string>(x => x.Contains("bob"))).Returns("1account");
        provider.ExternalCommunicationAdapter.PostApiResponseAsync(Arg.Any<string>(), Arg.Is<string>(x => x.Contains("1account"))).Returns("1session");
        var data = JsonSerializer.Serialize(new List<DexcomResult> { dexcomResult });
        provider.ExternalCommunicationAdapter.PostApiResponseAsync(Arg.Any<string>(), Arg.Is<string>(x => x.Contains("1session"))).Returns(data);
        provider.Runner.Process().Wait();
        return this;
    }

    public ReadBehaviorDriver GettingLatestNightScoutReading()
    {
        var data = JsonSerializer.Serialize(new List<NightScoutResult> { nightscoutResult });
        provider.ExternalCommunicationAdapter.GetApiResponseAsync(Arg.Any<string>()).Returns(data);
        provider.Runner.Process().Wait();
        return this;
    }

    public ReadBehaviorDriver CommunicationErrorOccurs()
    {
        provider.ExternalCommunicationAdapter.GetApiResponseAsync(Arg.Any<string>()).ThrowsAsync(x => throw new Exception());
        provider.ExternalCommunicationAdapter.PostApiResponseAsync(Arg.Any<string>()).ThrowsAsync(x => throw new Exception());
        provider.Runner.Process().Wait();
        return this;
    }

    public ReadAssertionDriver Then => new(provider, this);
}

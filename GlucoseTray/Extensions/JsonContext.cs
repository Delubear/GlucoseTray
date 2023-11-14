using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using static GlucoseTray.Services.GlucoseFetchService;

namespace GlucoseTray.Extensions
{
    //[JsonSerializable(typeof(string))]
    //[JsonSerializable(typeof(double))]
    //[JsonSerializable(typeof(long))]
    //[JsonSerializable(typeof(int))]
    //[JsonSerializable(typeof(bool))]
    //[JsonSerializable(typeof(FetchMethod))]
    //[JsonSerializable(typeof(DexcomServerLocation))]
    //[JsonSerializable(typeof(GlucoseUnitType))]
    [JsonSerializable(typeof(NightScoutStatus))]
    [JsonSerializable(typeof(AppSettingsContainer))]
    [JsonSerializable(typeof(GlucoseTraySettings))]
    [JsonSerializable(typeof(List<NightScoutResult>))]
    [JsonSerializable(typeof(List<DexcomResult>))]
    [JsonSerializable(typeof(List<DexComAccountIdJson>))]
    [JsonSerializable(typeof(List<DexComAccountIdJsonId>))]
    //[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
    public partial class GlucoseSourceGenerationContext : JsonSerializerContext
    {

        //public GlucoseSourceGenerationContext()
        //{
        //    Default.Options = new JsonSerializerOptions {  }
        //}
        //protected override JsonSerializerOptions? GeneratedSerializerOptions => throw new NotImplementedException();

        //public override JsonTypeInfo? GetTypeInfo(Type type)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

using System.ComponentModel;

namespace GlucoseTray.Enums;

internal enum DexcomServerLocation
{
    [Description("US Share 1")]
    DexcomShare1 = 0,

    [Description("US Share 2")]
    DexcomShare2 = 1,

    [Description("International")]
    DexcomInternational = 2,
}

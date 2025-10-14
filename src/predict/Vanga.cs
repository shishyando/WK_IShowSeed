
using System.Collections.Generic;

namespace IShowSeed.Prediction;

public static class Vanga
{
    public struct PerkMachinePrediction
    {
        public string Id;
        public List<string> PerkIds;
    }

    public struct RouteInfo {
        public string RouteName;
        public List<PerkMachinePrediction> PerkPreidctions;
    }

    public static List<RouteInfo> GenerateRouteInfos(int s)
    {
        IShowSeedPlugin.StartingSeed = s;
        return [];
    }
}

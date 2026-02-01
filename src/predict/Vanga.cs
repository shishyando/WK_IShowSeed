
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IShowSeed.Prediction;

public enum RouteType
{
    DEFAULT,
    SHORTCUT_SINK,
    SHORTCUT_BURNER,
}

public static class Vanga
{
    public struct RouteInfo {
        public string RouteName;
        public List<PerkMachinePred> PerkMachines;
        public Dictionary<string, IEnumerable<string>> VendoItems;
    }

    public static void DoSeedSearch()
    {
        if (!string.IsNullOrWhiteSpace(Plugin.DesiredRouteDescription.Value))
        {
            const int RequiredPerkCount = 3;

            var parts = Plugin.DesiredRouteDescription.Value.Split(':');
            if (parts.Length == 0)
            {
                Plugin.Beep.LogWarning($"Invalid DesiredRouteDescription format: '{Plugin.DesiredRouteDescription.Value}'. Expected format: '{{route}}: {{perk1}}, {{perk2}}, {{perk3}}'");
                return;
            }

            string routeName = parts[0].Trim().ToLower();
            string[] perks = parts.Length >= 2 ? [.. parts[1].Split(',').Select(p => p.Trim().ToLower())] : [];
            if (!Enum.TryParse(routeName.ToUpper(), out RouteType routeType))
            {
                Plugin.Beep.LogWarning($"Invalid desired route name: '{routeName}'. Valid routes are: {string.Join(", ", Enum.GetNames(typeof(RouteType)))}");
                return;
            }

            if (perks.Length != RequiredPerkCount)
            {
                Plugin.Beep.LogWarning($"Invalid desired perk count: {perks.Length}. Expected exactly {RequiredPerkCount} perks but got: {string.Join(", ", perks)}");
                return;
            }

            Plugin.Beep.LogInfo($"Searching desired route: {routeName}, perks: {string.Join(", ", perks)}");

            UnityEngine.Random.InitState(Plugin.SeedSearchIterations.Value);
            int foundSeeds = 0;
            for (int iteration = 0; iteration <= Plugin.SeedSearchIterations.Value; ++iteration)
            {
                int seed = UnityEngine.Random.Range(0, 10000000);
                RouteInfo prediction = GenerateRouteInfo(seed, routeType);
                if (prediction.PerkMachines[0].PredictedPerks.PerkIds.Contains(perks[0]) &&
                    prediction.PerkMachines[1].PredictedPerks.PerkIds.Contains(perks[1]) &&
                    prediction.PerkMachines[2].PredictedPerks.PerkIds.Contains(perks[2]))
                {
                    Plugin.Beep.LogInfo($"Found suitable seed: {seed}, injectors: >= {prediction.VendoItems.Values.Sum(items => items.Count(x => x == "injector"))}");
                    Plugin.Beep.LogInfo(JsonConvert.SerializeObject(prediction.VendoItems, Formatting.Indented));
                    if (++foundSeeds >= Plugin.SeedSearchResultsNeeded.Value)
                    {
                        break;
                    }
                }
            }
        }
    }
    
    public static RouteInfo GenerateRouteInfo(int seed, RouteType routeType)
    {
        Plugin.SeedForRandom = seed;
        return routeType switch
        {
            RouteType.DEFAULT => new RouteInfo()
            {
                RouteName = routeType.ToString(),
                PerkMachines = [
                    new PerkMachinePred(App_PerkPage.PerkPageType.unstable, "M1_Silos_SafeArea_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Silo_To_Pipeworks_01", 3, 3),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "M3_Habitation_Shaft_Intro", 3, 3),
                ],
                VendoItems = VendoPredictor.Generate(RouteType.DEFAULT),
            },
            RouteType.SHORTCUT_SINK => new RouteInfo()
            {
                RouteName = routeType.ToString(),
                PerkMachines = [
                    new PerkMachinePred(App_PerkPage.PerkPageType.unstable, "Campaign_Interlude_Sink_To_Pipeworks_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Sink_To_Pipeworks_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "M3_Habitation_Shaft_Intro", 3, 3),
                ],
                VendoItems = VendoPredictor.Generate(RouteType.SHORTCUT_SINK),
            },
            RouteType.SHORTCUT_BURNER => new RouteInfo()
            {
                RouteName = routeType.ToString(),
                PerkMachines = [
                    new PerkMachinePred(App_PerkPage.PerkPageType.unstable, "M1_Silos_SafeArea_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Silo_To_Pipeworks_01", 3, 3),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Chute_To_Habitation", 2, 2),
                ],
                VendoItems = VendoPredictor.Generate(RouteType.SHORTCUT_BURNER),
            },
            _ => new RouteInfo(),
        };
    }
}

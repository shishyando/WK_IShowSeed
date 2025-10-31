
using System.Collections.Generic;
using System.Linq;

namespace IShowSeed.Prediction;

public static class Vanga
{
    private static readonly List<string> _machines = [
        "perkpage_unstable_M1_Silos_SafeArea_01_2_2",
        "perkpage_regular_Campaign_Interlude_Silo_To_Pipeworks_01_3_3",
        "perkpage_regular_Campaign_Interlude_Sink_To_Pipeworks_01_2_2",
        "perkpage_unstable_Campaign_Interlude_Sink_To_Pipeworks_01_2_2",
        "perkpage_regular_M3_Habitation_Shaft_Intro_3_3",
        "perkpage_regular_Campaign_Interlude_Chute_To_Habitation_2_2",
    ];

    public struct PerkMachinePred
    {
        public App_PerkPage.PerkPageType PerkPageType;
        public string LevelName;
        public PredictedPerks PredictedPerks;

        public PerkMachinePred(App_PerkPage.PerkPageType type, string levelName, int minCards, int maxCards)
        {
            (PerkPageType, LevelName) = (type, levelName);
            PredictedPerks = PerkMock.Generate(type, levelName, minCards, maxCards);
        }
    }

    public struct RouteInfo {
        public string RouteName;
        public List<PerkMachinePred> PerkMachines;
    }

    public static List<RouteInfo> GenerateRouteInfos(int s)
    {
        Plugin.SeedForRandom = s;
        return [
            new RouteInfo()
            {
                RouteName = "default",
                PerkMachines = [
                    new PerkMachinePred(App_PerkPage.PerkPageType.unstable, "M1_Silos_SafeArea_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Silo_To_Pipeworks_01", 3, 3),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "M3_Habitation_Shaft_Intro", 3, 3),
                ]
            },
            new RouteInfo()
            {
                RouteName = "shortcut_sink",
                PerkMachines = [
                    new PerkMachinePred(App_PerkPage.PerkPageType.unstable, "Campaign_Interlude_Sink_To_Pipeworks_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Sink_To_Pipeworks_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "M3_Habitation_Shaft_Intro", 3, 3),
                ]
            },
            new RouteInfo()
            {
                RouteName = "shortcut_burner",
                PerkMachines = [
                    new PerkMachinePred(App_PerkPage.PerkPageType.unstable, "M1_Silos_SafeArea_01", 2, 2),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Silo_To_Pipeworks_01", 3, 3),
                    new PerkMachinePred(App_PerkPage.PerkPageType.regular, "Campaign_Interlude_Chute_To_Habitation", 2, 2),
                ]
            }
        ];
    }

    public static void DoSeedSearch()
    {
        if (!string.IsNullOrWhiteSpace(Plugin.DesiredRouteDescription.Value))
        {
            const int RequiredPerkCount = 3;
            var validRoutes = new HashSet<string> { "default", "shortcut_sink", "shortcut_burner" };

            var parts = Plugin.DesiredRouteDescription.Value.Split(':');
            if (parts.Length == 0)
            {
                Plugin.Beep.LogWarning($"Invalid DesiredRouteDescription format: '{Plugin.DesiredRouteDescription.Value}'. Expected format: '{{route}}: {{perk1}}, {{perk2}}, {{perk3}}'");
                return;
            }

            string routeName = parts[0].Trim().ToLower();
            string[] perks = parts.Length >= 2 ? [.. parts[1].Split(',').Select(p => p.Trim().ToLower())] : [];
            if (!validRoutes.Contains(routeName))
            {
                Plugin.Beep.LogWarning($"Invalid desired route name: '{routeName}'. Valid routes are: {string.Join(", ", validRoutes)}");
                return;
            }

            if (perks.Length != RequiredPerkCount)
            {
                Plugin.Beep.LogWarning($"Invalid desired perk count: {perks.Length}. Expected exactly {RequiredPerkCount} perks but got: {string.Join(", ", perks)}");
                return;
            }

            Plugin.Beep.LogInfo($"Searching desired route: {routeName}, perks: {string.Join(", ", perks)}");

            for (int seed = Plugin.SeedSearchMin.Value; seed <= Plugin.SeedSearchMax.Value; ++seed)
            {
                Vanga.RouteInfo prediction = GenerateRouteInfos(seed).First(x => x.RouteName == routeName);
                if (prediction.PerkMachines[0].PredictedPerks.PerkIds.Contains(perks[0]) &&
                    prediction.PerkMachines[1].PredictedPerks.PerkIds.Contains(perks[1]) &&
                    prediction.PerkMachines[2].PredictedPerks.PerkIds.Contains(perks[2]))
                {
                    Plugin.Beep.LogInfo($"Found suitable seed: {seed}");
                }
            }
        }
    }
}

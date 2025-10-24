
using System.Collections.Generic;
using IShowSeed.Random;

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
        Plugin.StartingSeed = s;
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
}

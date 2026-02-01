
using System.Collections.Generic;
using IShowSeed.Random;

namespace IShowSeed.Prediction;



public struct PredictedPerks
{
    public List<string> PerkIds;
    public List<string> RefreshedPerkIds;
}

public struct PerkMachinePred
{
    public App_PerkPage.PerkPageType PerkPageType;
    public string LevelName;
    public PredictedPerks PredictedPerks;

    public PerkMachinePred(App_PerkPage.PerkPageType type, string levelName, int minCards, int maxCards)
    {
        (PerkPageType, LevelName) = (type, levelName);
        PredictedPerks = PerkPredictor.Generate(type, levelName, minCards, maxCards);
    }
}

public static class PerkPredictor
{
    private static readonly List<string> _machines = [
        "perkpage_unstable_M1_Silos_SafeArea_01_2_2",
        "perkpage_regular_Campaign_Interlude_Silo_To_Pipeworks_01_3_3",
        "perkpage_regular_Campaign_Interlude_Sink_To_Pipeworks_01_2_2",
        "perkpage_unstable_Campaign_Interlude_Sink_To_Pipeworks_01_2_2",
        "perkpage_regular_M3_Habitation_Shaft_Intro_3_3",
        "perkpage_regular_Campaign_Interlude_Chute_To_Habitation_2_2",
    ];

    public static PredictedPerks Generate(App_PerkPage.PerkPageType perkPageType, string levelName, int minCards, int maxCards)
    {
        Rod.SwitchToMode(Rod.ERandomMode.Prediction);
        bool refresh = false;
        PredictedPerks predictedPerks = new()
        {
            PerkIds = [],
            RefreshedPerkIds = [],
        };

        for (int refreshIteration = 0; refreshIteration <= 1; ++refreshIteration, refresh = true)
        {
            Rod.Context _ = new();
            Rod.Enter(ref _, $"perkpage_{perkPageType}_{levelName}_{minCards}_{maxCards}_{refresh}");

            List<string> tmp;
            if (perkPageType == App_PerkPage.PerkPageType.regular)
                tmp = GenerateInternal(perkPageType, minCards, maxCards, refresh, predictedPerks.PerkIds, ["standard"]);
            else tmp = GenerateInternal(perkPageType, minCards, maxCards, refresh, predictedPerks.PerkIds, ["unstable"]);
            
            if (!refresh)
            {
                predictedPerks.PerkIds = [.. tmp];
            }
            else
            {
                predictedPerks.RefreshedPerkIds = [.. tmp];
            }

            Rod.Exit(in _);
        }

        Rod.SwitchToMode(Rod.ERandomMode.Disabled);
        
        return predictedPerks;
    }


    private static List<string> GenerateInternal(App_PerkPage.PerkPageType perkPageType, int minCards, int maxCards, bool refresh, List<string> generatedPerks, List<string> requiredPerkTags)
    {
        List<string> result = [];
        List<string> list = [];
        if (generatedPerks.Count > 0)
        {
            for (int i = generatedPerks.Count - 1; i >= 0; i--)
            {
                if (refresh)
                {
                    list.Add(generatedPerks[i]);
                }
            }
        }
        int num = UnityEngine.Random.Range(minCards, maxCards + 1);
        List<Perk> list2 = [.. Helpers.GetAllPerks()];
        int j = list2.Count - 1;
        while (j >= 0)
        {
            if (perkPageType == App_PerkPage.PerkPageType.regular)
            {
                if (list2[j].spawnPool == Perk.PerkPool.standard)
                {
                    goto IL_160;
                }
                list2.RemoveAt(j);
            }
            else
            {
                if (perkPageType != App_PerkPage.PerkPageType.unstable || list2[j].spawnPool == Perk.PerkPool.unstable)
                {
                    goto IL_160;
                }
                list2.RemoveAt(j);
            }
        IL_1AD:
            j--;
            continue;
        IL_160:
            if (true)
            {
                bool flag = false;
                foreach (string item in requiredPerkTags)
                {
                    if (list2[j].tags.Contains(item))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list2.RemoveAt(j);
                }
            }
            if (refresh && list2.Count > maxCards && list != null && list.Contains(list2[j].id))
            {
                list2.RemoveAt(j);
                goto IL_1AD;
            }
            goto IL_1AD;
        }
        for (int k = 0; k < num; k++)
        {
            bool flag = false;
            Perk perk = null;
            int num2 = 0;
            while (!flag && num2 < 100)
            {
                num2++;
                perk = list2[UnityEngine.Random.Range(0, list2.Count)];
                // if (!perk.CanSpawn())
                // {
                // }
                // else
                if (perkPageType == App_PerkPage.PerkPageType.unstable)
                {
                    flag = true;
                }
                else if ((k < num - 1 && perk.cost == 0) || (k == num - 1 && perk.cost > 0))
                {
                    flag = true;
                }
            }
            list2.Remove(perk);
            result.Add(perk.id);
        }
        return result;
    }
}

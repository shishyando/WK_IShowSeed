
using System.Collections.Generic;

namespace IShowSeed.Prediction;

public class PerkMock
{
    public List<string> GeneratedPerkIds = [];

    public void Reset()
    {
        GeneratedPerkIds.Clear();
    }

    public List<string> Generate(bool refresh, int minCards, int maxCards, App_PerkPage.PerkPageType perkPageType)
    {
        List<string> list = new List<string>();
        if (GeneratedPerkIds.Count > 0)
        {
            for (int i = GeneratedPerkIds.Count - 1; i >= 0; i--)
            {
                if (refresh)
                {
                    list.Add(GeneratedPerkIds[i]);
                }
            }
            GeneratedPerkIds.Clear();
        }
        int num = UnityEngine.Random.Range(minCards, maxCards + 1);
        List<Perk> list2 = new List<Perk>();
        list2.AddRange(CL_AssetManager.GetFullCombinedAssetDatabase().perkAssets);
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
                if (!perk.CanSpawn())
                {
                }
                else if (perkPageType == App_PerkPage.PerkPageType.unstable)
                {
                    flag = true;
                }
                else if ((k < num - 1 && perk.cost == 0) || (k == num - 1 && perk.cost > 0))
                {
                    flag = true;
                }
            }
            list2.Remove(perk);
            GeneratedPerkIds.Add(perk.id);
        }
        return GeneratedPerkIds;
    }
}

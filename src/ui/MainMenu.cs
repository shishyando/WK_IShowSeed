using System;
using HarmonyLib;
using IShowSeed.Prediction;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IShowSeed.Random.UI;

[PermanentPatch]
[HarmonyPatch(typeof(MenuManager), "Start")]
public static class MenuManager_Start_Patcher
{
    public static void Postfix(MenuManager __instance)
    {
        PatchSeedWindow(__instance);
        AddSeedWindowButton(__instance);
    }

    private static void AddSeedWindowButton(MenuManager __instance)
    {
        Transform logbookButton = __instance.menu.transform.Find("Main Menu Buttons/Logbook");
        Transform seedWindowButton = UnityEngine.Object.Instantiate(logbookButton, logbookButton.transform.parent);
        seedWindowButton.name = "Seeded runs";
        TextMeshProUGUI seedWindowText = seedWindowButton.GetChild(0).GetComponent<TextMeshProUGUI>();
        seedWindowText.text = "seeded runs";
        seedWindowText.color = Color.grey;
        seedWindowText.fontSize /= 2;
        Button button = seedWindowButton.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() =>
            {
                __instance.seedWindow.SetActive(!__instance.seedWindow.activeSelf);
            });
    }

    private static void PatchSeedWindow(MenuManager __instance)
    {
        GameObject seedWindow = __instance.seedWindow;
        TextMeshProUGUI title = seedWindow.transform.Find("Overview Titles/Title Text").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI buttonText = seedWindow.transform.Find("Tab Selection Hor/Exit/Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
        Button button = seedWindow.transform.Find("Tab Selection Hor/Exit").gameObject.GetComponent<Button>();
        TMP_InputField seedPrompt = seedWindow.transform.Find("Seed Input").gameObject.GetComponent<TMP_InputField>();
        TextMeshProUGUI placeholder = seedWindow.transform.Find("Seed Input/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>();
        if (buttonText == null || seedPrompt == null || button == null || title == null || placeholder == null)
        {
            Plugin.Beep.LogWarning($"button: {button}\nbuttonText: {buttonText}\nseedPrompt: {seedPrompt}\ntitle: {title}\nplaceholder {placeholder}");
            return;
        }
        PatchTitle(title);
        PatchPrompt(seedPrompt, placeholder);
        PatchButton(button);

        void PatchTitle(TextMeshProUGUI title)
        {
            title.text = "ENTER SEED";
        }

        void PatchPrompt(TMP_InputField prompt, TextMeshProUGUI placeholder)
        {
            prompt.onValueChanged = new TMP_InputField.OnChangeEvent();
            prompt.onValueChanged.AddListener(_ =>
            {
                buttonText.color = Color.white;
                buttonText.text = "Save to Config";
            });
            prompt.text = Plugin.ConfigPresetSeed.Value.ToString();
            if (prompt.text == "0")
            {
                prompt.text = "";
            }
            placeholder.text = "KEEP RANDOM";
        }

        void PatchButton(Button button)
        {
            buttonText.color = Color.grey;
            buttonText.text = "Save to Config";
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() =>
            {
                bool success = int.TryParse(seedPrompt.text, out int newSeed);
                if (success || seedPrompt.text == "")
                {
                    Plugin.ConfigPresetSeed.Value = newSeed;
                    Plugin.Instance.Config.Save();
                    Plugin.Beep.LogInfo($"Set new seed to {newSeed}");

                    // Plugin.Beep.LogInfo("Seed preview:");
                    // foreach (RouteType routeType in Enum.GetValues(typeof(RouteType)))
                    // {
                    //     Plugin.Beep.LogInfo($"{routeType}: {JsonConvert.SerializeObject(Vanga.GenerateRouteInfo(newSeed, routeType), Formatting.Indented)}");
                    // }
                    seedWindow.SetActive(false);
                }
                else
                {
                    buttonText.text = "Invalid seed";
                    buttonText.color = Color.red;
                }
            });
        }
    }

}

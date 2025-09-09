using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IShowSeed.Patches.UI;

[HarmonyPatch(typeof(MenuManager), "Start")]
public static class MenuManager_Start_Patcher
{

    [HarmonyPostfix]
    public static void PatchSeedWindow(MenuManager __instance)
    {
        GameObject seedWindow = __instance.seedWindow;
        TextMeshProUGUI title = seedWindow.transform.Find("Overview Titles/Title Text").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI buttonText = seedWindow.transform.Find("Tab Selection Hor/Exit/Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
        Button button = seedWindow.transform.Find("Tab Selection Hor/Exit").gameObject.GetComponent<Button>();
        TMP_InputField seedPrompt = seedWindow.transform.Find("Seed Input").gameObject.GetComponent<TMP_InputField>();
        TextMeshProUGUI placeholder = seedWindow.transform.Find("Seed Input/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>();
        if (buttonText == null || seedPrompt == null || button == null || title == null || placeholder == null)
        {
            IShowSeedPlugin.Logger.LogWarning($"button: {button}\nbuttonText: {buttonText}\nseedPrompt: {seedPrompt}\ntitle: {title}\nplaceholder {placeholder}");
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
            prompt.onValueChanged.AddListener((string _) =>
            {
                buttonText.color = Color.white;
                buttonText.text = "Save to Config";
            });
            prompt.text = IShowSeedPlugin.configPresetSeed.Value.ToString();
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
                    IShowSeedPlugin.configPresetSeed.Value = newSeed;
                    IShowSeedPlugin.Instance.Config.Save();
                    IShowSeedPlugin.Logger.LogInfo($"Set new seed to {newSeed}");
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

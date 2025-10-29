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
        // CreatePreviewButton(seedWindow, seedPrompt);

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
            // // Don't change position, just make it narrower
            // RectTransform buttonRect = button.GetComponent<RectTransform>();
            // if (buttonRect != null)
            // {
            //     float originalWidth = buttonRect.sizeDelta.x;
            //     float newWidth = originalWidth * 0.65f; // Make it 65% of original width
            //     buttonRect.sizeDelta = new Vector2(newWidth, buttonRect.sizeDelta.y);
            // }
            
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
                    Plugin.Beep.LogInfo($"Run preview: {JsonConvert.SerializeObject(Vanga.GenerateRouteInfos(newSeed), Formatting.Indented)}");
                    seedWindow.SetActive(false);
                }
                else
                {
                    buttonText.text = "Invalid seed";
                    buttonText.color = Color.red;
                }
            });
        }
        
        // void CreatePreviewButton(GameObject seedWindow, TMP_InputField seedPrompt)
        // {
        //     // Initialize RouteInfoWindow if not already done
            
        //     // Get the Exit button (which is now narrower)
        //     GameObject exitButton = seedWindow.transform.Find("Tab Selection Hor/Exit").gameObject;
        //     RectTransform exitRect = exitButton.GetComponent<RectTransform>();
            
        //     // Create preview button next to the Exit button
        //     GameObject previewButtonObj = Object.Instantiate(exitButton, exitButton.transform.parent);
        //     previewButtonObj.name = "PreviewButton";
            
        //     RectTransform previewRect = previewButtonObj.GetComponent<RectTransform>();
        //     previewRect.sizeDelta = new Vector2(exitRect.sizeDelta.x / 2, exitRect.sizeDelta.y);
            
        //     // Position it to the right of the Exit button with some spacing
        //     previewRect.anchoredPosition = exitRect.anchoredPosition + new Vector2(exitRect.sizeDelta.x + 1, 0);
            
        //     // Update button text to "preview"
        //     TextMeshProUGUI previewText = previewButtonObj.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
        //     if (previewText != null)
        //     {
        //         previewText.text = "preview";
        //     }
            
        //     // Set up button click handler
        //     Button previewButton = previewButtonObj.GetComponent<Button>();
        //     if (previewButton != null)
        //     {
        //         previewButton.onClick = new Button.ButtonClickedEvent();
        //         previewButton.onClick.AddListener(() =>
        //         {
        //             if (int.TryParse(seedPrompt.text, out int enteredSeed))
        //             {
                        
        //             }
        //         });
        //     }
        // }
    }

}

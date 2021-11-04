using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    // Not really how I would recommend doing localization. Only for demo purposes.
    [ExecuteInEditMode]
    public class DemoLocalizationScript : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        SystemLanguage[] languages;
        [SerializeField]
        string[] texts;
#pragma warning restore 0649

        Dictionary<SystemLanguage, string> textForLanguage;

        Text text;

        void Awake()
        {
            Setup();
        }

        void Setup()
        {
            if (text == null)
                text = GetComponent<Text>();
            if (text == null)
                Debug.LogError("No text component found.");

            if (textForLanguage == null)
                MergeArraysToDictionary();
        }

        void MergeArraysToDictionary()
        {
            int fullPairCount = languages.Length;
            if (languages.Length > texts.Length)
                Debug.LogError("Missing text for language.");
            else if (languages.Length < texts.Length)
            {
                fullPairCount = texts.Length;
                Debug.LogError("Missing language for text.");
            }

            textForLanguage = new Dictionary<SystemLanguage, string>();
            for (int i = 0; i < fullPairCount; ++i)
                textForLanguage[languages[i]] = texts[i];
        }

        void OnEnable()
        {
            MultiLangScreenshotScript.LanguageChanged += LanguageChanged;
            LanguageChanged(MultiLangScreenshotScript.currentLanguage);
        }

        void OnDisable()
        {
            MultiLangScreenshotScript.LanguageChanged -= LanguageChanged;
        }

        void LanguageChanged(SystemLanguage language)
        {
            Setup();

            text.text = textForLanguage[language].Replace(" - ", "\n\n-");
        }
    }
}
using System.Collections;
using UnityEngine;

namespace TRS.CaptureTool
{
    public class MultiLangScreenshotScript : ScreenshotSubComponentScript
    {
        public ScreenshotSeriesScript screenshotSeriesScript;

        public static System.Action<SystemLanguage> LanguageChanged;
        public static SystemLanguage currentLanguage = SystemLanguage.English;

        public SystemLanguage[] languages;

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.MultiLanguage; } }
#endif

        protected override void Awake()
        {
            base.Awake();

            if (screenshotSeriesScript == null)
                screenshotSeriesScript = GetComponentInParent<ScreenshotSeriesScript>();
            if (screenshotSeriesScript == null)
                screenshotSeriesScript = GetComponentInChildren<ScreenshotSeriesScript>();
        }

        public void TakeAllMultiLangScreenshots(bool save = true)
        {
            if (LanguageChanged == null)
                Debug.LogError("No listeners for language change event, so there would be no differences between screenshots.");
            else
                StartCoroutine(TakeScreenshotsForLanguage(screenshotScript.TakeAllScreenshotsCoroutine, save));
        }

        public void TakeMultiLangScreenshotSeries(bool save = true)
        {
            if (LanguageChanged == null)
                Debug.LogError("No listeners for language change event, so there would be no differences between screenshots.");
            if (screenshotSeriesScript == null)
                Debug.LogError("ScreenshotSeriesScript must be set to take screenshot series.");
            else
                StartCoroutine(TakeScreenshotsForLanguage(screenshotSeriesScript.TakeScreenshotSeriesCoroutine, save));
        }

        IEnumerator TakeScreenshotsForLanguage(ScreenshotScript.CaptureAndSaveRoutine captureAndSaveRoutine, bool save = true)
        {
            int originalCount = screenshotScript.fileSettings.count;
            bool originalIncludeLanguageInPath = screenshotScript.fileSettings.includeLanguageInPath;
            screenshotScript.fileSettings.includeLanguageInPath = true;

            foreach (SystemLanguage language in languages)
            {
                currentLanguage = language;
                if (LanguageChanged != null)
                    LanguageChanged(currentLanguage);

                screenshotScript.fileSettings.SetCount(originalCount);
                yield return StartCoroutine(captureAndSaveRoutine(save));

            }

            screenshotScript.fileSettings.includeLanguageInPath = originalIncludeLanguageInPath;
        }
    }
}
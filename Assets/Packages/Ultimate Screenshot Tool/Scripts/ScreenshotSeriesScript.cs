using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TRS.CaptureTool
{
    public class ScreenshotSeriesScript : ScreenshotSubComponentScript
    {
        public bool captureInitialScreen = true;
        public List<ButtonInteraction> buttonInteractions = new List<ButtonInteraction>();

        [System.Serializable]
        public struct ButtonInteraction
        {
            public Button button;
            public float animationDelay;
            public bool takePhoto;
        }

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.ScreenshotSeries; } }
#endif

        public void TakeScreenshotSeries(bool save = true)
        {
            StartCoroutine(TakeScreenshotSeriesCoroutine(save));
        }

        public IEnumerator TakeScreenshotSeriesCoroutine(bool save = true)
        {
            if (captureInitialScreen)
                yield return StartCoroutine(screenshotScript.TakeAllScreenshotsCoroutine(save));

            foreach (ButtonInteraction buttonInteraction in buttonInteractions)
            {
                if (buttonInteraction.button != null)
                    buttonInteraction.button.onClick.Invoke();

                yield return new WaitForSecondsRealtime(buttonInteraction.animationDelay);

                if (buttonInteraction.takePhoto)
                    yield return StartCoroutine(screenshotScript.TakeAllScreenshotsCoroutine(save));
            }
        }
    }
}
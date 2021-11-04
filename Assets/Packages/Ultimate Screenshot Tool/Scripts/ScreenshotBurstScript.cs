using System.Collections;
using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class ScreenshotBurstScript : ScreenshotSubComponentScript
    {
        public static System.Action WillTakeScreenshotBurst;
        public static System.Action ScreenshotBurstTaken;

        public HotKeySet takeScreenshotBurstKeySet = new HotKeySet { keyCode = KeyCode.B };

        public int burstSize = 10;
        public int skipFrames = 0;
#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.Burst; } }

#pragma warning disable 0414
        [SerializeField]
        bool showTakeAllScreenshotBurstButton;
#pragma warning restore 0414
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        void Update()
        {
            if (Input.anyKeyDown && !UIStatus.InputFieldFocused())
            {
                bool takeScreenshotBurst = takeScreenshotBurstKeySet.MatchesInput();
                if (screenshotScript.screenshotsInProgress && takeScreenshotBurst)
                {
                    Debug.Log("Screenshots already in progress.");
                    return;
                }

                if (takeScreenshotBurst)
                    TakeScreenshotBurst(true);
            }
        }
#endif

        public void TakeScreenshotBurst(bool save = true)
        {
            StartCoroutine(TakeScreenshotBurst(screenshotScript.TakeSingleScreenshotCoroutine, save));
        }

        public void TakeAllScreenshotBurst(bool save = true)
        {
            StartCoroutine(TakeScreenshotBurst(screenshotScript.TakeAllScreenshotsCoroutine, save));
        }

        IEnumerator TakeScreenshotBurst(ScreenshotScript.CaptureAndSaveRoutine captureAndSaveRoutine, bool save = true)
        {
            if (WillTakeScreenshotBurst != null)
                WillTakeScreenshotBurst();

            for (int i = 0; i < burstSize; ++i)
            {
                yield return StartCoroutine(captureAndSaveRoutine(save));
                for (int j = 0; j < skipFrames + 1; ++j)
                    yield return new WaitForEndOfFrame();
            }

            if (ScreenshotBurstTaken != null)
                ScreenshotBurstTaken();
        }
    }
}
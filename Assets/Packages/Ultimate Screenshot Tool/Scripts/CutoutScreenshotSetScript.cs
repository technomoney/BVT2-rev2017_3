using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class CutoutScreenshotSetScript : ScreenshotSubComponentScript
    {
        public static System.Action WillTakeCutoutSet;
        public static System.Action CutoutSetTaken;

        public HotKeySet takeCutoutSetKeySet = new HotKeySet { keyCode = KeyCode.N };

        public bool overrideResolution;
        public int resolutionWidth;
        public int resolutionHeight;

        public List<CutoutScript> cutoutScripts = new List<CutoutScript>();

        public bool overrideCutoutAdjustedRectTransforms;
        public List<RectTransform> cutoutAdjustedRectTransforms = new List<RectTransform>();

        Resolution originalResolution;
        List<RectTransform> originalCutoutAdjustedRectTransforms = new List<RectTransform>();

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.Burst; } }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        void Update()
        {
            if (Input.anyKeyDown && !UIStatus.InputFieldFocused())
            {
                bool takeCutoutSet = takeCutoutSetKeySet.MatchesInput();
                if (screenshotScript.screenshotsInProgress && takeCutoutSet)
                {
                    Debug.Log("Screenshots already in progress.");
                    return;
                }

                if (takeCutoutSet)
                    TakeCutoutSet(true);
            }
        }
#endif

        public void TakeCutoutSet(bool save = true)
        {
            StartCoroutine(TakeCutoutSet(screenshotScript.TakeSingleScreenshotCoroutine, save));
        }

        IEnumerator TakeCutoutSet(ScreenshotScript.CaptureAndSaveRoutine captureAndSaveRoutine, bool save = true)
        {
            if (WillTakeCutoutSet != null)
                WillTakeCutoutSet();

            if (overrideResolution)
            {
                Resolution resolution = new Resolution { width = resolutionWidth, height = resolutionHeight };
                originalResolution = ScreenExtensions.CurrentResolution();
                bool resolutionIsDifferent = !originalResolution.IsSameSizeAs(resolution);
                if (resolutionIsDifferent)
                {
                    ScreenExtensions.UpdateResolution(resolution);
                    yield return new WaitForResolutionUpdates();
                }
            }

            if (overrideCutoutAdjustedRectTransforms)
            {
                originalCutoutAdjustedRectTransforms = screenshotScript.cutoutAdjustedRectTransforms;
                screenshotScript.cutoutAdjustedRectTransforms = cutoutAdjustedRectTransforms;
            }

            CutoutScript originalCutoutScript = screenshotScript.cutoutScript;
            bool originalPreviewCutoutScript = originalCutoutScript.preview;
            originalCutoutScript.preview = false;

            List<bool> originalPreviewCutoutScripts = new List<bool>();
            foreach (CutoutScript cutoutScript in cutoutScripts)
            {
                originalPreviewCutoutScripts.Add(cutoutScript.preview);
                cutoutScript.preview = false;
            }

            foreach (CutoutScript cutoutScript in cutoutScripts)
            {
                screenshotScript.cutoutScript = cutoutScript;
                yield return StartCoroutine(captureAndSaveRoutine(save));
            }

            for (int i = 0; i < cutoutScripts.Count; ++i)
                cutoutScripts[i].preview = originalPreviewCutoutScripts[i];

            screenshotScript.cutoutScript = originalCutoutScript;
            screenshotScript.cutoutScript.preview = originalPreviewCutoutScript;

            if (overrideCutoutAdjustedRectTransforms)
                screenshotScript.cutoutAdjustedRectTransforms = originalCutoutAdjustedRectTransforms;

            if (overrideResolution)
            {
                Resolution resolution = new Resolution { width = resolutionWidth, height = resolutionHeight };
                bool resolutionIsDifferent = !originalResolution.IsSameSizeAs(resolution);
                if (resolutionIsDifferent)
                {
                    ScreenExtensions.UpdateResolution(originalResolution);
                    yield return new WaitForResolutionUpdates();
                }
            }

            if (CutoutSetTaken != null)
                CutoutSetTaken();
        }
    }
}
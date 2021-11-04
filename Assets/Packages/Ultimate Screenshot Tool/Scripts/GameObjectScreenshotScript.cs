using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    public class GameObjectScreenshotScript : ScreenshotSubComponentScript
    {
        [System.Serializable]
        public class GameObjectResolutionPair
        {
            public GameObject gameObject;
            public ScreenshotResolution resolution;
        }

        public List<GameObject> gameObjects = new List<GameObject>();
        public List<ScreenshotResolution> screenshotResolutions = new List<ScreenshotResolution>();
        public List<GameObjectResolutionPair> pairs = new List<GameObjectResolutionPair>();

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.GameObject; } }
#endif

        // Migrates old separate list into new pair object for clearer UI
        public GameObjectScreenshotScript()
        {
            if (gameObjects.Count != 0)
            {
                for (int i = 0; i < gameObjects.Count; ++i)
                {
                    int resolutionIndex = Mathf.Min(i, screenshotResolutions.Count - 1);
                    GameObjectResolutionPair pair = new GameObjectResolutionPair { gameObject = gameObjects[i], resolution = screenshotResolutions[resolutionIndex] };
                    pairs.Add(pair);
                }
            }

            gameObjects.Clear();
            screenshotResolutions.Clear();
        }

        public void TakeGameObjectScreenshots(bool save = true)
        {
            StartCoroutine(CaptureGameObjects(save));
        }

        IEnumerator CaptureGameObjects(bool save = true)
        {
            int originalScreenshotIndex = screenshotScript.fileSettings.count;
            List<ScreenshotResolution> originalScreenshotResolutions = screenshotScript.screenshotResolutions;
            for (int i = 0; i < gameObjects.Count; ++i)
            {
                screenshotScript.fileSettings.SetCount(originalScreenshotIndex);
                int resolutionIndex = Mathf.Min(i, screenshotResolutions.Count - 1);
                screenshotScript.screenshotResolutions = new List<ScreenshotResolution>() { screenshotResolutions[resolutionIndex] };
                gameObjects[i].SetActive(true);
                // Need to force wait for updates here to catch a corner case. 
                //(If original canvas size is in list of resolutions, then TakeAllScreenshotsCoroutine will resize back to it at end of method.
                // the next run will think the resolution has already been set/adjusted, and it won't wait for adjustments)
                yield return StartCoroutine(screenshotScript.TakeAllScreenshotsCoroutine(save));
                gameObjects[i].SetActive(false);
            }
            screenshotScript.screenshotResolutions = originalScreenshotResolutions;
            screenshotScript.fileSettings.SetCount(originalScreenshotIndex + 1);
            screenshotScript.fileSettings.SaveCount();
        }
    }
}
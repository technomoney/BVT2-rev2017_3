using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_5_4_OR_NEWER
#endif

// Saves to Mobile with: https://github.com/yasirkula/UnityNativeGallery

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    [CanEditMultipleObjects]
    [DisallowMultipleComponent]
#endif
    [System.Serializable]
    public sealed class ScreenshotScript : CaptureScript
    {
        public const string version = "2.06";
        const string TRS_SCREENSHOTS_MAX_INSTANCES_KEY = "TRS_SCREENSHOTS_MAX_INSTANCES_KEY";

        public static System.Action WillTakeMultipleScreenshots;
        public static System.Action<int, int, int> WillTakeScreenshot;
        public static System.Action<Texture2D> ScreenshotTaken;
        public static System.Action<string> ScreenshotSaved;
        public static System.Action MultipleScreenshotsTaken;

        [System.Serializable]
        public enum ScreenshotCaptureMode
        {
            RenderTexture,
            ScreenCapture
        };

        [System.Serializable]
        public enum ResolutionSelect
        {
            GameViewResolution,
            DefaultResolution,
            CameraResolution
        };

        static bool maxInstancesLoaded;
        static int cachedMaxInstances = 1;
        static List<ScreenshotScript> instances = new List<ScreenshotScript>();

        public List<ScreenshotResolution> screenshotResolutions = new List<ScreenshotResolution>();

        public HotKeySet takeSingleScreenshotKeySet = new HotKeySet { keyCode = KeyCode.F };
        public HotKeySet takeAllScreenshotsKeySet = new HotKeySet { keyCode = KeyCode.V };

        public bool playScreenshotAudioInGame = true;
        public AudioSource screenshotAudioSource;

        public ScreenshotFileSettings fileSettings = new ScreenshotFileSettings();
        public Texture2D lastScreenshotTexture;
        [System.Obsolete("Please use lastFileSavePath instead.")]
        public string lastScreenshotFilePath { get { return lastSaveFilePath; } set { lastSaveFilePath = value; } }

        // Advance Settings
        public ScreenshotCaptureMode captureMode = ScreenshotCaptureMode.ScreenCapture;
        public ResolutionSelect sceneViewScreenshotResolution = ResolutionSelect.CameraResolution;

        public bool screenshotsInProgress { get; private set; }
#pragma warning disable 0649
        bool screenCaptureAvailable;
#pragma warning restore 0649
        public override bool useCanvasesAdjuster
        {
            get
            {
                return autoSwitchRenderMode && uiCamera != null && captureMode == ScreenshotCaptureMode.RenderTexture;
            }
        }

        #region Editor variables
        public delegate IEnumerator CaptureAndSaveRoutine(bool save = true);
#if UNITY_EDITOR
        public System.WeakReference[] subComponents = new System.WeakReference[(int)SubComponentType.Size];
        public enum SubComponentType
        {
            ScreenshotSeries,
            GameObject,
            MultiLanguage,
            Burst,
            CutoutSet,
            Size
        }

        public ScreenshotSubComponentScript GetSubComponent(SubComponentType subComponentType)
        {
            System.WeakReference weakSubComponent = subComponents[(int)subComponentType];
            if (weakSubComponent == null || !weakSubComponent.IsAlive)
            {
                subComponents[(int)subComponentType] = null;
                return null;
            }

            return subComponents[(int)subComponentType].Target as ScreenshotSubComponentScript;
        }

        public void SetSubComponent(SubComponentType subComponentType, ScreenshotSubComponentScript subComponent)
        {
            if (subComponent == null)
            {
                subComponents[(int)subComponentType] = null;
                return;
            }

            subComponent.subWindowMode = true;
            subComponent.hiddenMode = subComponent.gameObject == gameObject;
            subComponents[(int)subComponentType] = new System.WeakReference(subComponent);
        }

        public override void RefreshSubComponents()
        {
            base.RefreshSubComponents();

            ScreenshotSubComponentScript[] subComponentScripts = GetComponents<ScreenshotSubComponentScript>();
            foreach (ScreenshotSubComponentScript subComponentScript in subComponentScripts)
            {
                if (subComponentScript is ScreenshotSeriesScript)
                    SetSubComponent(SubComponentType.ScreenshotSeries, subComponentScript);
                else if (subComponentScript is GameObjectScreenshotScript)
                    SetSubComponent(SubComponentType.GameObject, subComponentScript);
                else if (subComponentScript is MultiLangScreenshotScript)
                    SetSubComponent(SubComponentType.MultiLanguage, subComponentScript);
                else if (subComponentScript is ScreenshotBurstScript)
                    SetSubComponent(SubComponentType.Burst, subComponentScript);
                else if (subComponentScript is CutoutScreenshotSetScript)
                    SetSubComponent(SubComponentType.CutoutSet, subComponentScript);
            }
        }

        public bool adjustScale;
        public bool adjustDelay;
        public bool playScreenshotAudioInEditor = true;
        [SerializeField]
        bool shareResolutionsBetweenPlatforms = true;
        [SerializeField] // Serialized to preserve values
        ScreenshotResolutionSet[] screenshotResolutionsForType = new ScreenshotResolutionSet[20]; // Leaving some wiggle room
        GameViewSizeGroupType currentType;
#pragma warning disable 0414
        [SerializeField]
        int screenshotsEditorRefreshHack;
        [SerializeField]
        bool showResolutionList = true;
        [SerializeField]
        bool showResolutionSettings;
#pragma warning restore 0414
#endif
        #endregion

        protected override void Awake()
        {
            base.Awake();

#if UNITY_2017_3_OR_NEWER
            screenCaptureAvailable = true;
#endif
            fileSettings.SetUp(gameObject.GetInstanceID());
        }

        protected override void Start()
        {
            base.Start();

            screenshotsInProgress = false;
#if UNITY_EDITOR
            UpdateScreenshotResolutions();
#endif
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        protected override void Update()
        {
            base.Update();
#if UNITY_EDITOR
            if (editorWindowMode)
                return;
#endif
            if (Input.anyKeyDown && !UIStatus.InputFieldFocused())
            {
                bool takeSingleScreenshot = takeSingleScreenshotKeySet.MatchesInput();
                bool takeAllScreenshots = takeAllScreenshotsKeySet.MatchesInput();
                if (screenshotsInProgress && (takeSingleScreenshot || takeAllScreenshots))
                {
                    Debug.Log("Screenshots already in progress.");
                    return;
                }

                if (takeSingleScreenshot)
                    TakeSingleScreenshot(true);
                if (takeAllScreenshots)
                    TakeAllScreenshots(true);
            }
        }
#endif

        public void TakeAllScreenshots(bool save = true)
        {
            StartCoroutine(TakeAllScreenshotsCoroutine(save));
        }

        public IEnumerator TakeAllScreenshotsCoroutine(bool save = true)
        {
            screenshotsInProgress = true;
#if UNITY_EDITOR
            PrepareGameViewResolutions();
#endif
            if (WillTakeMultipleScreenshots != null)
                WillTakeMultipleScreenshots();

            CleanUpCameraList();
            PrepareToCapture();
            Camera[] camerasToUse = cameras.ToArray();
            ScreenshotResolution[] resolutionsToUse = screenshotResolutions.ToArray();

            bool fixedResolutionDevice = false;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            fixedResolutionDevice = true;
#endif


#if UNITY_EDITOR
            int originalSelectedSizeIndex = GameView.GetSelectedSizeIndex();
#else
            Resolution originalResolution = ScreenExtensions.CurrentResolution();
#endif

            yield return new WaitForSecondsRealtime(delayBeforeCapture);

            // Resolve all remaining updates
            yield return new WaitForEndOfFrame();

            Resolution currentResolution = ScreenExtensions.CurrentResolution();
            if (resolutionsToUse.Length < 1 || fixedResolutionDevice)
                resolutionsToUse = new ScreenshotResolution[] { new ScreenshotResolution(true, NameForResolution(currentResolution), currentResolution.width, currentResolution.height, 1) };
            foreach (ScreenshotResolution screenshotResolution in resolutionsToUse)
            {
                if (!screenshotResolution.active)
                    continue;

                // Necessary to get current resolution to see if we need to switch
                currentResolution = ScreenExtensions.CurrentResolution();
                bool screenCaptureMode = captureMode == ScreenshotCaptureMode.ScreenCapture || camerasToUse.Length <= 0;
                Resolution screenResolutionToUse = screenCaptureAvailable || !screenCaptureMode ? screenshotResolution.unscaledResolution : screenshotResolution.scaledResolution;
                bool resolutionIsDifferent = !screenResolutionToUse.IsSameSizeAs(currentResolution);
                if (resolutionIsDifferent)
                {
                    ScreenExtensions.UpdateResolution(screenResolutionToUse);
                    if (screenshotResolution.waitForUpdates)
                        yield return new WaitForResolutionUpdates();
                }

                if (screenshotResolution.delay > 0)
                {
                    if (stopTimeDuringCapture)
                        Time.timeScale = 1f;
                    yield return new WaitForSeconds(screenshotResolution.delay);
                    if (stopTimeDuringCapture)
                        Time.timeScale = 0f;
                }

                // Yield to be sure ScreenCapture occurs at right time, that any ResolutionUpdated events have taken place and generally that the resizing is finished
                yield return new WaitForEndOfFrame();

                string cameraName = (camerasToUse.Length > 0 && camerasToUse[0] != null) ? camerasToUse[0].name : "CameraName";
                string resolutionName = screenshotResolution.name;
                string resolutionString = screenshotResolution.resolutionString;
                if (WillTakeScreenshot != null)
                    WillTakeScreenshot(screenshotResolution.width, screenshotResolution.height, screenshotResolution.scale);

                bool solidify = fileSettings.fileType == ScreenshotFileSettings.FileType.PNG && !fileSettings.allowTransparency;
                Texture2D screenshotTexture = Screenshot(camerasToUse, screenshotResolution.unscaledResolution, screenshotResolution.scale, solidify);

                MonoBehaviourExtended.FlexibleDestroy(lastScreenshotTexture);
                lastScreenshotTexture = screenshotTexture;
                if (ScreenshotTaken != null)
                    ScreenshotTaken(lastScreenshotTexture);

                if (save)
                {
                    string fullFilePath = fileSettings.FullFilePathWithCaptureDetails(cameraName, resolutionName, resolutionString);
                    if (useCutout && cutoutScript != null)
                        fullFilePath = fileSettings.FullFilePath(cutoutScript.name, fileSettings.FileNameWithCaptureDetails(cameraName, resolutionString));
                    Save(screenshotTexture, fullFilePath, false);
                }
            }

            if (save)
            {
                fileSettings.IncrementCount();
                fileSettings.SaveCount();
            }

#if UNITY_EDITOR
            if (GameView.GetSelectedSizeIndex() != originalSelectedSizeIndex)
                GameView.SetSelecedSizeIndex(originalSelectedSizeIndex);
#else
            currentResolution = ScreenExtensions.CurrentResolution();
            if (currentResolution.width != originalResolution.width || currentResolution.height != originalResolution.height)
                ScreenExtensions.UpdateResolution(originalResolution);
#endif
            RestoreAfterCapture();
            if (MultipleScreenshotsTaken != null)
                MultipleScreenshotsTaken();

#if UNITY_EDITOR
            screenshotsEditorRefreshHack = 10;
#endif
            screenshotsInProgress = false;
        }

#if UNITY_EDITOR
        public void TakeSceneViewScreenshot(bool save = true)
        {
            StartCoroutine(SafeTakeSceneViewScreenshot(save));
        }

        IEnumerator SafeTakeSceneViewScreenshot(bool save = true)
        {
            ScreenshotCaptureMode originalCaptureMode = captureMode;
            captureMode = ScreenshotCaptureMode.RenderTexture;

            Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;
            yield return StartCoroutine(TakeScreenshotWithCameras(new Camera[] { sceneViewCamera }, sceneViewScreenshotResolution, save));

            captureMode = originalCaptureMode;
        }
#endif

        public void TakeSingleScreenshot(bool save = true)
        {
            StartCoroutine(TakeSingleScreenshotCoroutine(save));
        }

        public IEnumerator TakeSingleScreenshotCoroutine(bool save = true)
        {
            CleanUpCameraList();
            ResolutionSelect resolutionSelect = captureMode == ScreenshotCaptureMode.ScreenCapture ? ResolutionSelect.GameViewResolution : ResolutionSelect.CameraResolution;
            yield return StartCoroutine(TakeScreenshotWithCameras(cameras.ToArray(), resolutionSelect, save));
        }

        IEnumerator TakeScreenshotWithCameras(Camera[] camerasToUse, ResolutionSelect resolutionSelect, bool save = true, bool waitForEndOfFrame = false)
        {
            screenshotsInProgress = true;
            PrepareToCapture();
            Resolution resolution = new Resolution();
            if (resolutionSelect == ResolutionSelect.CameraResolution)
                resolution = new Resolution { width = camerasToUse[0].pixelWidth, height = camerasToUse[0].pixelHeight };
            else if (resolutionSelect == ResolutionSelect.DefaultResolution && screenshotResolutions.Count > 0)
                resolution = new Resolution { width = screenshotResolutions[0].width, height = screenshotResolutions[0].height };
            else
                resolution = ScreenExtensions.CurrentResolution();

            int scale = 1;
            string cameraName = (camerasToUse.Length > 0 && camerasToUse[0] != null) ? camerasToUse[0].name : "CameraName";
            string resolutionName = NameForResolution(resolution);
            string resolutionString = new ScreenshotResolution(true, resolutionName, resolution, scale).resolutionString;
            bool screenCaptureMode = captureMode == ScreenshotCaptureMode.ScreenCapture || camerasToUse.Length <= 0;
            if (WillTakeScreenshot != null)
                WillTakeScreenshot(resolution.width, resolution.height, scale);

            yield return new WaitForSecondsRealtime(delayBeforeCapture);

            if (waitForEndOfFrame || screenCaptureMode)
                yield return new WaitForEndOfFrame();

            bool solidify = fileSettings.fileType == ScreenshotFileSettings.FileType.PNG && !fileSettings.allowTransparency;
            Texture2D screenshotTexture = Screenshot(camerasToUse, resolution, scale, solidify);
            MonoBehaviourExtended.FlexibleDestroy(lastScreenshotTexture);
            lastScreenshotTexture = screenshotTexture;
            if (ScreenshotTaken != null)
                ScreenshotTaken(lastScreenshotTexture);

            if (save)
            {
                string fullFilePath = fileSettings.FullFilePathWithCaptureDetails(cameraName, resolutionName, resolutionString);
                if (useCutout && cutoutScript != null)
                    fullFilePath = fileSettings.FullFilePath(cutoutScript.name, fileSettings.FileNameWithCaptureDetails(cameraName, resolutionString));
                Save(screenshotTexture, fullFilePath, false);
                fileSettings.IncrementCount();
                fileSettings.SaveCount();
            }

            RestoreAfterCapture();
            screenshotsInProgress = false;
        }

        public Texture2D Screenshot(Camera[] camerasToUse, Resolution resolution, int scale, bool solidify)
        {
#if UNITY_EDITOR
            if (playScreenshotAudioInEditor && screenshotAudioSource != null && screenshotAudioSource.clip != null)
                screenshotAudioSource.Play();
#else
            if (playScreenshotAudioInGame && screenshotAudioSource != null && screenshotAudioSource.clip != null)
                screenshotAudioSource.Play();
#endif

            Texture2D screenshotTexture = null;
            if (captureMode == ScreenshotCaptureMode.RenderTexture && camerasToUse.Length > 0)
                screenshotTexture = CombinedRenderTextures(camerasToUse, resolution, scale, TextureFormat.ARGB32, 24, false);
            else
            {
                if (captureMode != ScreenshotCaptureMode.ScreenCapture)
                    Debug.LogError("Forced to use Screen Capture mode as cameras are not set.");

                screenshotTexture = FlexibleScreenCaptureWithCutout(resolution, scale, TextureFormat.ARGB32, false);
            }

            if (solidify)
                screenshotTexture.Solidify(false);
            screenshotTexture.Apply(false);

            return screenshotTexture;
        }

        public void Save(Texture2D texture, string fullFilePath, bool destroyTexture)
        {
            byte[] bytes = null;
            if (fileSettings.fileType == ScreenshotFileSettings.FileType.PNG)
                bytes = texture.EncodeToPNG();
            else if (fileSettings.fileType == ScreenshotFileSettings.FileType.JPG)
                bytes = texture.EncodeToJPG(fileSettings.jpgQuality);

            bool persist = true;
#if !UNITY_EDITOR
#if UNITY_IOS || UNITY_ANDROID
            persist &= fileSettings.persistLocallyMobile;
#elif UNITY_WEBGL
            persist &= fileSettings.persistLocallyWeb;
#endif
#endif
#pragma warning disable
            bool encounteredError = false;
#pragma warning restore
            if (persist)
            {
                try
                {
                    System.IO.File.WriteAllBytes(fullFilePath, bytes);
                }
                catch (System.Exception e)
                {
                    encounteredError = true;
                    Debug.LogError("Encountered exception attempting to save texture (or screenshot): " + e);
                }
            }
            else
                fullFilePath = null;

#if !UNITY_EDITOR
#if UNITY_IOS || UNITY_ANDROID
            if (fileSettings.saveToGallery)
            {
                string fileName = Application.productName.Replace(" ", "") + "{0}" + fileSettings.extension;
                if (persist)
                    NativeGallery.SaveImageToGallery(fullFilePath, fileSettings.album, fileName, (error) => { if(error != null && error.ToString().Length >0) {  Debug.LogError("Error: " + error); } });
                else
                    NativeGallery.SaveImageToGallery(bytes, fileSettings.album, fileName, (error) => { if(error != null && error.ToString().Length >0) {  Debug.LogError("Error: " + error); } });
            }
#elif UNITY_WEBGL
            Process(bytes, fileSettings);
#endif
#endif
            lastSaveFilePath = fullFilePath;
            if (ScreenshotSaved != null)
                ScreenshotSaved(fullFilePath);
            if (destroyTexture)
                MonoBehaviourExtended.FlexibleDestroy(texture);

#if UNITY_EDITOR
            if (persist && !encounteredError)
                Debug.Log("Screenshot saved: " + fullFilePath);
#endif
        }

        #region Helpers
        #region Screenshot Resolution Helpers
#if UNITY_EDITOR
        void PrepareGameViewResolutions()
        {
            foreach (ScreenshotResolution screenshotResolution in screenshotResolutions)
            {
                cameras.RemoveAll(camera => camera == null);
                bool screenCaptureMode = captureMode == ScreenshotCaptureMode.ScreenCapture || cameras.Count <= 0;
                Resolution screenResolutionToUse = screenCaptureAvailable || !screenCaptureMode ? screenshotResolution.unscaledResolution : screenshotResolution.scaledResolution;
                if (!GameView.SizeExists(screenResolutionToUse))
                    GameView.AddTempCustomSize(GameView.GameViewSizeType.FixedResolution, screenResolutionToUse);
            }
        }
#endif

        string NameForResolution(Resolution resolution)
        {
            foreach (ScreenshotResolution screenshotResolution in screenshotResolutions)
            {
                if (screenshotResolution.unscaledResolution.IsSameSizeAs(resolution))
                    return screenshotResolution.name;
            }

#if UNITY_EDITOR
            return AdditionalResolutions.ConvertToStructuredFolderName(GameView.NameForSize(resolution));
#endif
#pragma warning disable 0162
            return "";
#pragma warning restore 0162
        }
        #endregion

        #region Scene Change Helpers
        public override void UpdateDontDestroyOnLoad()
        {
            bool willDestroy = false;
            if (Application.isPlaying && dontDestroyOnLoad)
            {
                if (instances.Count < GetMaxInstances())
                {
                    instances.Add(this);
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    MonoBehaviourExtended.FlexibleDestroy(gameObject);
                    willDestroy = true;
                }
            }

            if (!willDestroy)
                base.UpdateDontDestroyOnLoad();
        }
        #endregion

        #region Clean Up Inputs

        public void ScaleResolutionsToScreen()
        {
            for (int i = 0; i < screenshotResolutions.Count; ++i)
            {
                ScreenshotResolution screenshotResolution = screenshotResolutions[i];
                screenshotResolution.ScaleToScreen();
                screenshotResolutions[i] = screenshotResolution;
            }
        }
        #endregion

        #region Editor Variable Change Updates
#if UNITY_EDITOR
        public void UpdateScreenshotResolutions()
        {
            if (!shareResolutionsBetweenPlatforms)
            {
                currentType = GameView.GetCurrentGroupType();
                if (screenshotResolutionsForType[(int)currentType] == null)
                    screenshotResolutions = new List<ScreenshotResolution>(screenshotResolutions);
                else
                    screenshotResolutions = screenshotResolutionsForType[(int)currentType].screenshotResolutions;
            }
        }

        public void ScreenshotResolutionsChanged()
        {
            if (!shareResolutionsBetweenPlatforms)
            {
                currentType = GameView.GetCurrentGroupType();
                if (screenshotResolutionsForType[(int)currentType] == null)
                    screenshotResolutionsForType[(int)currentType] = new ScreenshotResolutionSet(currentType.ToString(), new List<ScreenshotResolution>(screenshotResolutions));
                else
                    screenshotResolutionsForType[(int)currentType].screenshotResolutions = new List<ScreenshotResolution>(screenshotResolutions);
            }
        }

        public void EditorDirectoryChanged()
        {
            fileSettings.SaveEditorDirectory();
        }
#endif
        #endregion

        #region Max Instances Count
        public override int GetMaxInstances()
        {
            if (maxInstancesLoaded)
                return cachedMaxInstances;
            return LoadMaxInstances();
        }

        public override void SetMaxInstances(int newValue)
        {
            maxInstancesLoaded = true;
            cachedMaxInstances = newValue;
            PlayerPrefs.SetInt(TRS_SCREENSHOTS_MAX_INSTANCES_KEY, newValue);
            PlayerPrefs.Save();
        }

        static int LoadMaxInstances()
        {
            if (PlayerPrefs.HasKey(TRS_SCREENSHOTS_MAX_INSTANCES_KEY))
                cachedMaxInstances = PlayerPrefs.GetInt(TRS_SCREENSHOTS_MAX_INSTANCES_KEY);
            else
                cachedMaxInstances = 1;
            maxInstancesLoaded = true;
            return cachedMaxInstances;
        }
        #endregion
        #endregion
    }
}
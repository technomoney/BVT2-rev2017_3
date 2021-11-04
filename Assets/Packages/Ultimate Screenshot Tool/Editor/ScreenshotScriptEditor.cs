using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ScreenshotScript))]
    public class ScreenshotScriptEditor : CaptureScriptEditor
    {
        ReorderableList resolutionList;

        ScreenshotSeriesScriptEditor screenshotSeriesScriptEditor;
        GameObjectScreenshotScriptEditor gameObjectScreenshotScriptEditor;
        MultiLangScreenshotScriptEditor multiLangScreenshotScriptEditor;
        ScreenshotBurstScriptEditor screenshotBurstScriptEditor;
        CutoutScreenshotSetScriptEditor cutoutScreenshotSetScriptEditor;

        // Force repaint if we need to update the preview frame or the recording or save progress bar
        public override bool RequiresConstantRepaint()
        {
            return ((ScreenshotScript)target).screenshotsInProgress;
        }

        protected override void OnEnable()
        {
            if (target == null)
                return;

            base.OnEnable();

            CreateScreenshotResolutionList();
        }

        #region Tabs
        protected override void CaptureTab()
        {
            Resolutions();

            CutoutSetting();

            if (cutoutScreenshotSetScriptEditor != null)
                cutoutScreenshotSetScriptEditor.Settings();
            if (screenshotBurstScriptEditor != null)
                screenshotBurstScriptEditor.Settings();
            if (multiLangScreenshotScriptEditor != null)
                multiLangScreenshotScriptEditor.Settings();
            if (screenshotSeriesScriptEditor != null)
                screenshotSeriesScriptEditor.Settings();
            if (gameObjectScreenshotScriptEditor != null)
                gameObjectScreenshotScriptEditor.Settings();

            TempEnabledObjects();

            TempDisabledObjects();

            Timing(true);

            EditorGUILayout.Space();

            CaptureButtons();

            EditorGUILayout.Space();

            OpenFileOrFolderButtons();
        }

        protected override void SaveTab()
        {
            SaveSettings();
        }

        protected override void SettingsTab()
        {
            CaptureMode();

            ScreenshotScript.ScreenshotCaptureMode captureMode = (ScreenshotScript.ScreenshotCaptureMode)serializedObject.FindProperty("captureMode").intValue;
            if (captureMode != ScreenshotScript.ScreenshotCaptureMode.ScreenCapture)
                Cameras();

            if (captureMode != ScreenshotScript.ScreenshotCaptureMode.ScreenCapture)
                UICamera(serializedObject.FindProperty("fileSettings").FindPropertyRelative("saveType").stringValue);

            Background(true);

            if (!((ScreenshotScript)target).editorWindowMode)
                HotKeys();

            AudioSettings();
            MouseSettings();
            ResolutionSettings();
            DontDestroyOnLoadSettings();

            SupportButtons();
        }
        #endregion

        #region Capture Tab Settings
        void Resolutions()
        {
            bool showResolutionList = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showResolutionList", "Resolutions");
            if (showResolutionList)
            {
                EditorGUI.BeginChangeCheck();
                resolutionList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                    localEditorUpdateQueue.Enqueue(((ScreenshotScript)target).ScreenshotResolutionsChanged);

                /*
                                if (serializedObject.FindProperty("adjustScale").boolValue)
                                {
                                    if (GUILayout.Button("Scale Resolutions to Screen"))
                                    {
                                        Undo.RecordObject(target, "Scaled resolutions to screen");
                                        ((ScreenshotScript)target).ScaleResolutionsToScreen();
                                    }
                                }
                                EditorGUILayout.Space();

                */
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("None"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = false;
                    }
                }

                if (GUILayout.Button("Portrait"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = screenshotResolution.height >= screenshotResolution.width;
                    }
                }

                if (GUILayout.Button("Landscape"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = screenshotResolution.width >= screenshotResolution.height;
                    }
                }

                if (GUILayout.Button("All"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = true;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void CaptureButtons()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !((ScreenshotScript)target).screenshotsInProgress;
            bool allowSceneViewScreenshot = SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null;
            if (allowSceneViewScreenshot)
            {
                if (GUILayout.Button("Take SceneView Screenshot", GUILayout.MinHeight(40)))
                {
                    RequestSavePath(((ScreenshotScript)target).EditorDirectoryChanged);
                    ((ScreenshotScript)target).TakeSceneViewScreenshot(true);
                }
            }

            if (GUILayout.Button("Take GameView Screenshot", GUILayout.MinHeight(40)))
            {
                if (GameView.CurrentGameViewSizeType() == GameView.GameViewSizeType.AspectRatio)
                    Debug.LogWarning("GameView has an Aspect Ratio size. Use a Fixed Resolution GameView size to capture your preferred resolution.");

                RequestSavePath(((ScreenshotScript)target).EditorDirectoryChanged);
                ((ScreenshotScript)target).TakeSingleScreenshot(true);
            }

            if (cutoutScreenshotSetScriptEditor != null)
                cutoutScreenshotSetScriptEditor.TakeCutoutSetButton();

            if (screenshotBurstScriptEditor != null)
                screenshotBurstScriptEditor.TakeBurstButton();

            if (GUILayout.Button("Take All Screenshots", GUILayout.MinHeight(60)))
            {
                RequestSavePath(((ScreenshotScript)target).EditorDirectoryChanged);
                ((ScreenshotScript)target).TakeAllScreenshots(true);
            }

            if (screenshotBurstScriptEditor != null)
                screenshotBurstScriptEditor.TakeAllBurstButton();

            if (multiLangScreenshotScriptEditor != null)
                multiLangScreenshotScriptEditor.TakeAllButton();
            if (screenshotSeriesScriptEditor != null)
                screenshotSeriesScriptEditor.Button();
            if (multiLangScreenshotScriptEditor != null)
                multiLangScreenshotScriptEditor.TakeSeriesButton();
            if (gameObjectScreenshotScriptEditor != null)
                gameObjectScreenshotScriptEditor.Button();

            GUI.enabled = originalGUIEnabled;
        }
        #endregion

        #region Save Tab Settings
        void SaveSettings()
        {
            bool showSaveSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSaveSettings", "Save Settings");
            if (showSaveSettings)
            {
                SerializedProperty fileSettingsProperty = serializedObject.FindProperty("fileSettings");
                FileSettings(((ScreenshotScript)target).fileSettings, fileSettingsProperty, ((ScreenshotScript)target).EditorDirectoryChanged);
            }
        }

        protected override void FileSettings(CaptureFileSettings fileSettings, SerializedProperty fileSettingsProperty, System.Action editorDirectoryChangedAction, bool includeUseStreamingAssets = false)
        {
            base.FileSettings(fileSettings, fileSettingsProperty, editorDirectoryChangedAction, includeUseStreamingAssets);

            EditorGUILayout.BeginHorizontal();
            ScreenshotFileSettings.FileType fileType = (ScreenshotFileSettings.FileType)fileSettingsProperty.FindPropertyRelative("fileType").intValue;

            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(fileType == ScreenshotFileSettings.FileType.PNG, "PNG", toggleStyle);
            if (EditorGUI.EndChangeCheck())
                fileSettingsProperty.FindPropertyRelative("fileType").intValue = (int)ScreenshotFileSettings.FileType.PNG;

            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(fileType == ScreenshotFileSettings.FileType.JPG, "JPG", toggleStyle);
            if (EditorGUI.EndChangeCheck())
                fileSettingsProperty.FindPropertyRelative("fileType").intValue = (int)ScreenshotFileSettings.FileType.JPG;
            EditorGUILayout.EndHorizontal();

            if (fileSettingsProperty.FindPropertyRelative("fileType").intValue == (int)ScreenshotFileSettings.FileType.JPG)
            {
                int currentQuality = fileSettingsProperty.FindPropertyRelative("jpgQuality").intValue;
                fileSettingsProperty.FindPropertyRelative("jpgQuality").intValue = EditorGUILayout.IntSlider("JPG Quality", currentQuality, 1, 100);
            }
            else
            {
                CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("allowTransparency"));
                EditorGUILayout.HelpBox("With Unity's standard alpha blending, projects with transparency will create partially transparent screenshots. Leave unchecked to ensure an alpha of 1. (See ReadMe to learn more and get the effect you want.)", MessageType.Info);
            }

            CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("includeLanguageInPath"));
        }
        #endregion

        #region Settings Tab Settings
        void CaptureMode()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Capture Mode");
            ScreenshotScript.ScreenshotCaptureMode captureMode = (ScreenshotScript.ScreenshotCaptureMode)serializedObject.FindProperty("captureMode").intValue;
            serializedObject.FindProperty("captureMode").intValue = (int)((ScreenshotScript.ScreenshotCaptureMode)EditorGUILayout.EnumPopup(captureMode));
            EditorGUILayout.EndHorizontal();

            captureMode = (ScreenshotScript.ScreenshotCaptureMode)serializedObject.FindProperty("captureMode").intValue;
            if (captureMode == ScreenshotScript.ScreenshotCaptureMode.RenderTexture)
                EditorGUILayout.HelpBox("Combines the render textures of the selected cameras. More complex. More customizable.", MessageType.Info);
            else
                EditorGUILayout.HelpBox("Takes screenshot of exactly what is visible on screen. Simpler to use. Less customizable.", MessageType.Info);
        }

        void HotKeys()
        {
            bool showHotKeys = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showHotKeys", "HotKeys");
            if (showHotKeys)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("takeSingleScreenshotKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("takeAllScreenshotsKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("previewCutoutKeySet"), true);

                EditorGUILayout.HelpBox("Hotkeys that overlap existing Unity Editor hotkeys can cause issues.", MessageType.Info);
                if (GUILayout.Button("Existing Unity Hotkeys"))
                    Application.OpenURL("https://docs.unity3d.com/Manual/UnityHotkeys.html");
            }
        }

        void ResolutionSettings()
        {
            bool showResolutionSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showResolutionSettings", "Resolution Settings");
            if (showResolutionSettings)
            {
                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("adjustDelay"));
                if (EditorGUI.EndChangeCheck())
                    CreateScreenshotResolutionList();
                EditorGUILayout.HelpBox("Enable to allow a delay before taking a screenshot. Useful if need time for animations to occur between screenshots such as when rotating between portrait and landscape.", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("adjustScale"));
                if (EditorGUI.EndChangeCheck())
                    CreateScreenshotResolutionList();
                EditorGUILayout.HelpBox("Only necessary if you want extremely large screenshots or get a warning similar to: 'GameView reduced to a reasonable size for this system'.", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                // Uses label and toggle for spacing reasons
                EditorGUILayout.LabelField("Share Resolutions Between Platforms");
                serializedObject.FindProperty("shareResolutionsBetweenPlatforms").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("shareResolutionsBetweenPlatforms").boolValue);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                    localEditorUpdateQueue.Enqueue(((ScreenshotScript)target).ScreenshotResolutionsChanged);

                EditorGUILayout.HelpBox("Check this box if you want the same screenshot resolutions on all platforms. Useful if your game doesn't have code/features that depend on the platform.", MessageType.Info);

                ScreenshotScript.ResolutionSelect currentResolutionSelect = (ScreenshotScript.ResolutionSelect)serializedObject.FindProperty("sceneViewScreenshotResolution").intValue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("SceneView Screenshot Resolution");
                serializedObject.FindProperty("sceneViewScreenshotResolution").intValue = (int)((ScreenshotScript.ResolutionSelect)EditorGUILayout.EnumPopup(currentResolutionSelect));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Camera Resolution uses the current camera resolution, so the screenshot looks exactly as you see it in the view.\n\nGameView Resolution is the currently selected GameView resolution.\n\nDefault Resolution will use the first item in the resolution list (or the GameView resolution if none exists).", MessageType.Info);

            }
        }

        void AudioSettings()
        {
            bool showAudioSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showAudioSettings", "Audio Settings");
            if (showAudioSettings)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("screenshotAudioSource"), new GUIContent("Screenshot Sound"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("playScreenshotAudioInEditor"), new GUIContent("Play in Editor"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("playScreenshotAudioInGame"), new GUIContent("Play in Game"));
            }
        }
        #endregion

        #region Helpers
        void CreateScreenshotResolutionList()
        {
            resolutionList = new ScreenshotResolutionReorderableList(serializedObject,
                                                         serializedObject.FindProperty("screenshotResolutions"),
                                                         serializedObject.FindProperty("adjustScale").boolValue,
                                                         serializedObject.FindProperty("adjustDelay").boolValue,
                                     true, true, true, true, serializedObject.FindProperty("shareResolutionsBetweenPlatforms"));

            if (gameObjectScreenshotScriptEditor == null)
                UpdateSubEditors();

            if (gameObjectScreenshotScriptEditor != null)
                gameObjectScreenshotScriptEditor.ReloadPairList(serializedObject.FindProperty("adjustScale").boolValue,
                                                               serializedObject.FindProperty("adjustDelay").boolValue);
        }

        protected override void GeneralUpdates()
        {
            ForceUpdates();
        }

        protected override void UpdateSubEditors()
        {
            base.UpdateSubEditors();

            ScreenshotSeriesScript screenshotSeriesScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.ScreenshotSeries) as ScreenshotSeriesScript;
            if (screenshotSeriesScript != null)
            {
                if (screenshotSeriesScriptEditor == null)
                    screenshotSeriesScriptEditor = (ScreenshotSeriesScriptEditor)Editor.CreateEditor(screenshotSeriesScript);
            }
            else
                screenshotSeriesScriptEditor = null;

            GameObjectScreenshotScript gameObjectScreenshotScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.GameObject) as GameObjectScreenshotScript;
            if (gameObjectScreenshotScript != null)
            {
                if (gameObjectScreenshotScriptEditor == null)
                    gameObjectScreenshotScriptEditor = (GameObjectScreenshotScriptEditor)Editor.CreateEditor(gameObjectScreenshotScript);
            }
            else
                gameObjectScreenshotScriptEditor = null;

            MultiLangScreenshotScript multiLangScreenshotScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.MultiLanguage) as MultiLangScreenshotScript;
            if (multiLangScreenshotScript != null)
            {
                if (multiLangScreenshotScriptEditor == null)
                    multiLangScreenshotScriptEditor = (MultiLangScreenshotScriptEditor)Editor.CreateEditor(multiLangScreenshotScript);
            }
            else
                multiLangScreenshotScriptEditor = null;

            ScreenshotBurstScript screenshotBurstScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.Burst) as ScreenshotBurstScript;
            if (screenshotBurstScript != null)
            {
                if (screenshotBurstScriptEditor == null)
                    screenshotBurstScriptEditor = (ScreenshotBurstScriptEditor)Editor.CreateEditor(screenshotBurstScript);
            }
            else
                screenshotBurstScriptEditor = null;

            CutoutScreenshotSetScript cutoutScreenshotSetScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.CutoutSet) as CutoutScreenshotSetScript;
            if (cutoutScreenshotSetScript != null)
            {
                if (cutoutScreenshotSetScriptEditor == null)
                    cutoutScreenshotSetScriptEditor = (CutoutScreenshotSetScriptEditor)Editor.CreateEditor(cutoutScreenshotSetScript);
            }
            else
                cutoutScreenshotSetScriptEditor = null;
        }

        void ForceUpdates()
        {
            if (((ScreenshotScript)target).screenshotsInProgress)
                serializedObject.FindProperty("screenshotsEditorRefreshHack").intValue += 1;
            else if (serializedObject.FindProperty("screenshotsEditorRefreshHack").intValue > 0)
                serializedObject.FindProperty("screenshotsEditorRefreshHack").intValue -= 1;
        }
        #endregion
    }
}
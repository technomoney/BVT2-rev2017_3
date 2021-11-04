using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

using TRS.CaptureTool.Share;
using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
#if UNITY_2017_2_OR_NEWER
    [InitializeOnLoadAttribute]
#endif
    public class CaptureScriptEditor : Editor
    {
        protected const int maxErrorLogs = 50;

        protected enum Tab
        {
            Capture,
            Save,
            Share,
            Settings,
            Size
        }
        protected Tab selectedTab;
        protected Tab[] tabAtIndex;
        protected string[] tabNames;
        protected bool allowsEdit = false;

        protected ReorderableList cameraList;
        protected ReorderableList tempEnabledList;
        protected ReorderableList tempDisabledList;
        protected ReorderableList cutoutAdjustedRectTransformsList;

        protected Queue<System.Action> localEditorUpdateQueue = new Queue<System.Action>();

        protected ShareScriptEditor shareScriptEditor;
        protected List<string> errorLogs = new List<string>();

        protected const string toggleStyle = "ToolbarButton";

        protected virtual void OnEnable()
        {
            if (target == null)
                return;

            cameraList = new ReorderableList(serializedObject,
                                 serializedObject.FindProperty("cameras"),
                                 true, true, true, true);
            cameraList.AddHeader("Camera");
            cameraList.AddStandardElementCallback();

            tempEnabledList = new ReorderableList(serializedObject,
                                 serializedObject.FindProperty("tempEnabledObjects"),
                                 true, true, true, true);
            tempEnabledList.AddHeader("Temporarily Enabled Object (Logo overlay, debug info, etc.)");
            tempEnabledList.AddStandardElementCallback();

            tempDisabledList = new ReorderableList(serializedObject,
                     serializedObject.FindProperty("tempDisabledObjects"),
                     true, true, true, true);
            tempDisabledList.AddHeader("Temporarily Disabled Object (Busy UI, debug info, etc.)");
            tempDisabledList.AddStandardElementCallback();

            cutoutAdjustedRectTransformsList = new ReorderableList(serializedObject,
                     serializedObject.FindProperty("cutoutAdjustedRectTransforms"),
                     true, true, true, true);
            cutoutAdjustedRectTransformsList.AddHeader("Adjusted Rect Transforms");
            cutoutAdjustedRectTransformsList.AddStandardElementCallback();

            Application.logMessageReceived += HandleLog;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
#endif

            ((CaptureScript)target).RefreshSubComponents();
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
#endif
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Log)
            {
                if (errorLogs.Count >= maxErrorLogs)
                    errorLogs.RemoveAt(0);
                errorLogs.Add(logString + "\n" + stackTrace);
            }
        }

#if UNITY_2017_2_OR_NEWER
        void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
                SelectTabIndex(0);
        }
#endif

        public override void OnInspectorGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            GeneralUpdates();
            UpdateSubEditors();

            EditorGUILayout.Space();

            TabSelect();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginVertical("Box");
            if (selectedTab == Tab.Capture)
                CaptureTab();
            else if (selectedTab == Tab.Save)
                SaveTab();
            else if (selectedTab == Tab.Share)
                ShareTab();
            else if (selectedTab == Tab.Settings)
                SettingsTab();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            while (localEditorUpdateQueue.Count > 0)
                ((CaptureScript)target).editorUpdateQueue.Enqueue(localEditorUpdateQueue.Dequeue());

            GUI.enabled = true;
        }

        void TabSelect()
        {
            int oldSelectedTabIndex = serializedObject.FindProperty("selectedTabIndex").intValue;
            int selectedTabIndex = GUILayout.Toolbar(oldSelectedTabIndex, tabNames);

            if (selectedTabIndex != oldSelectedTabIndex || selectedTab != tabAtIndex[selectedTabIndex])
                SelectTabIndex(selectedTabIndex);
        }

        protected void SelectTab(Tab tab)
        {
            for (int i = 0; i < tabAtIndex.Length; ++i)
            {
                if (tab == tabAtIndex[i])
                {
                    SelectTabIndex(i);
                    return;
                }
            }
        }

        protected void SelectTabIndex(int tabIndex)
        {
            serializedObject.FindProperty("selectedTabIndex").intValue = tabIndex;
            selectedTab = tabAtIndex[tabIndex];
            serializedObject.ApplyModifiedProperties();
        }

        #region Tab Stubs
        protected virtual void CaptureTab()
        {
            throw new UnityException("Capture Tab not implemented");
        }

        protected virtual void SaveTab()
        {
            throw new UnityException("Save Tab not implemented");
        }

        protected virtual void ShareTab()
        {
            if (shareScriptEditor != null)
                shareScriptEditor.FullShareUI();
        }

        protected virtual void SettingsTab()
        {
            throw new UnityException("Settings Tab not implemented");
        }
        #endregion

        #region Capture Tab Settings
        protected void TempEnabledObjects()
        {
            bool showEnabledObjects = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showEnabledObjects", "Extra Capture List");
            if (showEnabledObjects)
                tempEnabledList.DoLayoutList();
        }

        protected void TempDisabledObjects()
        {
            bool showDisabledObjects = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showDisabledObjects", "Do Not Capture List");
            if (showDisabledObjects)
                tempDisabledList.DoLayoutList();
        }

        protected void Timing(bool allowStopTime)
        {
            bool showTiming = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showTiming", "Timing");
            if (showTiming)
            {
                if (allowStopTime)
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("stopTimeDuringCapture"), new GUIContent("Stop Time to Capture"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("delayBeforeCapture"));

                if (Application.isPlaying)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Time Scale Override");
                    serializedObject.FindProperty("timeScaleOverride").floatValue = EditorGUILayout.Slider(serializedObject.FindProperty("timeScaleOverride").floatValue, 0f, 2f);
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                        localEditorUpdateQueue.Enqueue(((CaptureScript)target).TimeScaleOverrideChanged);
                }
            }
        }
        #endregion

        #region Save Tab Settings
        protected virtual void FileSettings(CaptureFileSettings fileSettings, SerializedProperty fileSettingsProperty, System.Action editorDirectoryChangedAction, bool includeUseStreamingAssets = false)
        {
            string saveType = fileSettingsProperty.FindPropertyRelative("saveType").stringValue;
            if (saveType.Length <= 0)
            {
                if (includeUseStreamingAssets)
                    fileSettingsProperty.FindPropertyRelative("saveType").stringValue = "Gifs";
                else
                    fileSettingsProperty.FindPropertyRelative("saveType").stringValue = "Screenshots";
                saveType = fileSettingsProperty.FindPropertyRelative("saveType").stringValue;
            }

            string saveTypeSingular = saveType;
            if (saveTypeSingular.EndsWith("s", System.StringComparison.Ordinal))
                saveTypeSingular = saveTypeSingular.Substring(0, saveTypeSingular.Length - 1); // remove s

            GUILayout.Label("Save Path", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            string currentDirectory = fileSettingsProperty.FindPropertyRelative("cachedEditorDirectory").stringValue;
            fileSettingsProperty.FindPropertyRelative("cachedEditorDirectory").stringValue = EditorGUILayout.TextField(currentDirectory, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
                OpenFolderPanelForSavePath(fileSettingsProperty, editorDirectoryChangedAction);
            EditorGUILayout.EndHorizontal();

            string folderHelpText = "A folder name ending with ~ (like /" + saveType + "~) will be ignored by Unity and is useful for excluding files and avoiding reimports when switching builds.";
            if (includeUseStreamingAssets)
                folderHelpText = "Gifs must be saved to the StreamingAssets folder to be loaded from a save file." + folderHelpText;
            EditorGUILayout.HelpBox(folderHelpText, MessageType.Info);

            if (!((CaptureScript)target).editorWindowMode)
            {
                bool showStandaloneSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showStandaloneSettings", "Standalone Save Settings");
                if (showStandaloneSettings)
                {
                    if (includeUseStreamingAssets)
                    {
                        CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("useStreamingAssetsPath"));
                        if (fileSettingsProperty.FindPropertyRelative("useStreamingAssetsPath").boolValue)
                            EditorGUILayout.HelpBox("All directories are relative to the streaming asset path. (Required to load a gif from a save file.)", MessageType.Info);
                        else
                            EditorGUILayout.HelpBox("All directories are relative to the data path. (Gifs may not be loaded from a save file.)", MessageType.Info);
                    }

                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("linuxDirectory"));
                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("macDirectory"));
                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("windowsDirectory"));

                    if (!includeUseStreamingAssets)
                        EditorGUILayout.HelpBox("All directories are relative to the data path.", MessageType.Info);
                }
                bool showMobileSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showMobileSettings", "Mobile Save Settings");
                if (showMobileSettings)
                {
                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("saveToGallery"));
                    if (fileSettingsProperty.FindPropertyRelative("saveToGallery").boolValue)
                    {
                        CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("androidAlbum"));
                        CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("iosAlbum"), new GUIContent("iOS Album"));

                        EditorGUILayout.HelpBox("Filename is the product name with a numerical identifier.", MessageType.Info);
                    }

                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("persistLocallyMobile"), new GUIContent("Persist Locally"));
                    if (fileSettingsProperty.FindPropertyRelative("persistLocallyMobile").boolValue)
                    {
                        CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("androidDirectory"));
                        CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("iosDirectory"), new GUIContent("iOS Directory"));

                        EditorGUILayout.HelpBox("Persist to use the " + saveTypeSingular.ToLower() + " later. Please remember to delete unused " + saveType.ToLower() + " from the user's device.\n\nDirectories are relative to the persistant data path.", MessageType.Info);
                    }
                }

                bool showWebSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showWebSettings", "Web Save Settings");
                if (showWebSettings)
                {
                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("openInNewTab"));
                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("download"));
                    if (fileSettingsProperty.FindPropertyRelative("download").boolValue)
                        CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("webFileName"), new GUIContent("File Name"));

                    CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("persistLocallyWeb"), new GUIContent("Persist Locally"));
                    if (fileSettingsProperty.FindPropertyRelative("persistLocallyWeb").boolValue)
                    {
                        CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("webDirectory"));

                        EditorGUILayout.HelpBox("Persist to use the " + saveTypeSingular.ToLower() + " later. Please remember to delete unused " + saveType.ToLower() + " from the user's device.\n\nDirectories are relative to the persistant data path.", MessageType.Info);
                    }
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("File Name Settings", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Example File Name:     " + fileSettings.ExampleFileName());

            CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("prefix"), new GUIContent("Custom Prefix"));

            Color originalColor = GUI.color;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            fileSettingsProperty.FindPropertyRelative("includeProject").boolValue = GUILayout.Toggle(fileSettingsProperty.FindPropertyRelative("includeProject").boolValue, "Project", toggleStyle);
            fileSettingsProperty.FindPropertyRelative("includeCamera").boolValue = GUILayout.Toggle(fileSettingsProperty.FindPropertyRelative("includeCamera").boolValue, "Camera", toggleStyle);
            fileSettingsProperty.FindPropertyRelative("includeDate").boolValue = GUILayout.Toggle(fileSettingsProperty.FindPropertyRelative("includeDate").boolValue, "Date", toggleStyle);
            fileSettingsProperty.FindPropertyRelative("includeResolution").boolValue = GUILayout.Toggle(fileSettingsProperty.FindPropertyRelative("includeResolution").boolValue, "Resolution", toggleStyle);
            fileSettingsProperty.FindPropertyRelative("includeCounter").boolValue = GUILayout.Toggle(fileSettingsProperty.FindPropertyRelative("includeCounter").boolValue, "Counter", toggleStyle);
            if (EditorGUI.EndChangeCheck())
                fileSettingsProperty.FindPropertyRelative("fileNameSettingsChanged").boolValue = true;
            GUI.color = originalColor;
            if (fileSettingsProperty.FindPropertyRelative("includeCounter").boolValue)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Reset Counter", toggleStyle))
                {
                    fileSettings.ResetCount();
                    fileSettings.SaveCount();
                }
            }

            EditorGUILayout.EndHorizontal();
            if (!fileSettingsProperty.FindPropertyRelative("fileNameSettingsChanged").boolValue)
                EditorGUILayout.HelpBox("Toggle the above tabs to add or remove that component from the filename. (And to dismiss this message.)", MessageType.Info);

            EditorGUILayout.Space();

            if (fileSettingsProperty.FindPropertyRelative("includeDate").boolValue)
            {
                EditorGUILayout.Space();
                CustomEditorGUILayout.PropertyField(fileSettingsProperty.FindPropertyRelative("dateFormat"));
            }
        }

        protected void RequestSavePath(System.Action editorDirectoryChangedAction)
        {
            SerializedProperty fileSettings = serializedObject.FindProperty("fileSettings");
            string currentDirectory = fileSettings.FindPropertyRelative("cachedEditorDirectory").stringValue;
            if (currentDirectory == "")
                OpenFolderPanelForSavePath(fileSettings, editorDirectoryChangedAction);
        }

        protected void OpenFolderPanelForSavePath(SerializedProperty fileSettings, System.Action editorDirectoryChangedAction)
        {
            string saveType = fileSettings.FindPropertyRelative("saveType").stringValue;
            string currentDirectory = fileSettings.FindPropertyRelative("cachedEditorDirectory").stringValue;
            string newDirectory = EditorUtility.SaveFolderPanel("Path to " + saveType + " Taken in Editor", currentDirectory, Application.dataPath);
            if (newDirectory.Length > 0)
                fileSettings.FindPropertyRelative("cachedEditorDirectory").stringValue = newDirectory;
            serializedObject.ApplyModifiedProperties();

            ((CaptureScript)target).editorUpdateQueue.Enqueue(editorDirectoryChangedAction);
            EditorGUIUtility.ExitGUI();
        }

        protected void OpenFileOrFolderButtons()
        {
            string filePath = serializedObject.FindProperty("lastSaveFilePath").stringValue;
            SerializedProperty captureFileSettingsProperty = serializedObject.FindProperty("fileSettings");
            string saveType = captureFileSettingsProperty.FindPropertyRelative("saveType").stringValue;
            saveType = saveType.Substring(0, saveType.Length - 1); // remove extra s
            string folderPath = captureFileSettingsProperty.FindPropertyRelative("cachedEditorDirectory").stringValue;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !string.IsNullOrEmpty(filePath);
            if (GUILayout.Button("View Last " + saveType, GUILayout.MinHeight(40)))
            {
                Application.OpenURL("file:///" + System.Uri.EscapeUriString(filePath));
                Debug.Log("Opening File " + filePath);
            }

            GUI.enabled = originalGUIEnabled && !string.IsNullOrEmpty(folderPath);
            if (GUILayout.Button("View " + saveType + "s Folder", GUILayout.MinHeight(60)))
            {
                Application.OpenURL("file:///" + System.Uri.EscapeUriString(folderPath));
                Debug.Log("Opening Directory " + folderPath);
            }
            GUI.enabled = originalGUIEnabled;
        }
        #endregion

        #region Settings Tab Settings
        protected void Cameras(bool multiCameraMode = true)
        {
            bool showCameraList = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showCameraList", "Cameras");
            if (showCameraList)
            {
                EditorGUI.BeginChangeCheck();
                if (multiCameraMode)
                    cameraList.DoLayoutList();
                else
                {
                    Camera camera = ((CaptureScript)target).cameras[0];
                    ((CaptureScript)target).cameras[0] = EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true) as Camera;
                    if (((CaptureScript)target).cameras[0] != camera)
                        EditorUtility.SetDirty(target);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed camera");
                    localEditorUpdateQueue.Enqueue(((CaptureScript)target).AnyCameraChanged);
                }

                if (GUILayout.Button("Add All Active Cameras"))
                {
                    ((CaptureScript)target).AddAllActiveCameras();
                    EditorUtility.SetDirty(target);
                }
            }
        }

        protected void Background(bool withFoldout = false)
        {
            bool showBackground = true;
            if (withFoldout)
                showBackground = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showBackground", "Background");
            if (showBackground)
            {

                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("overrideBackground"));

                if (serializedObject.FindProperty("overrideBackground").boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundCamera"));
                    if (EditorGUI.EndChangeCheck())
                        localEditorUpdateQueue.Enqueue(((CaptureScript)target).BackgroundCameraChanged);

                    if (!serializedObject.FindProperty("backgroundCameraSelected").boolValue)
                        EditorGUILayout.HelpBox("Currently selected camera is the camera with lowest depth. It may not be right. (Select a camera to dismiss this message.)", MessageType.Info);

                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundColor"));
                }
            }
        }

        protected void UICamera(string saveType)
        {
            bool showUICamera = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showUICamera", "UI Camera");
            if (showUICamera)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Autoswitch Render Mode");
                serializedObject.FindProperty("autoSwitchRenderMode").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("autoSwitchRenderMode").boolValue);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Experimental: Switches canvases from overlay mode to camera mode during " + saveType.ToLower() + ".", MessageType.Info);

                if (serializedObject.FindProperty("autoSwitchRenderMode").boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                    Camera currentUICamera = serializedObject.FindProperty("uiCamera").objectReferenceValue as Camera;
                    serializedObject.FindProperty("uiCamera").objectReferenceValue = EditorGUILayout.ObjectField("UI Camera", currentUICamera, typeof(Camera), true);
                    if (EditorGUI.EndChangeCheck())
                        localEditorUpdateQueue.Enqueue(((CaptureScript)target).UICameraChanged);

                    string uiCameraString = "The camera to display overlay UI on. A built-in camera with the preferred settings is used by default. However, additional cameras does result in slower rendering, so this option may be changed for optimization purposes.";
                    EditorGUILayout.HelpBox(uiCameraString, MessageType.Info);

                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("overridePlaneDistance"));
                    if (serializedObject.FindProperty("overridePlaneDistance").boolValue)
                    {
                        CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("planeDistanceOverride"));
                        EditorGUILayout.HelpBox("Plane distance to use for the canvas during " + saveType.ToLower() + ". Leave as 0 to keep the default value.", MessageType.Info);
                    }
                    else
                        EditorGUILayout.HelpBox("Plane distance to use for the canvases. No need to override if using default UI camera.", MessageType.Info);

                    if (GUILayout.Button("Refresh Canvas List", GUILayout.MinHeight(20)))
                        ((CaptureScript)target).RefeshCanvasList();
                    EditorGUILayout.HelpBox("The canvas list used by auto switch render mode may become out of date if new canvases are added. Refresh to get a new list.", MessageType.Info);
                }
            }
        }

        protected void CutoutSetting()
        {
            bool showCutout = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showCutout", "Cutout");
            if (showCutout)
            {
                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useCutout"));
                bool useCutout = serializedObject.FindProperty("useCutout").boolValue;
                if (EditorGUI.EndChangeCheck())
                {
                    if (!useCutout)
                    {
                        if (((CaptureScript)target).cutoutScript != null)
                            ((CaptureScript)target).cutoutScript.preview = false;
                        EditorUtility.SetDirty(target);
                    }
                }
                if (useCutout)
                {
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("cutoutScript"));
                    cutoutAdjustedRectTransformsList.DoLayoutList();
                    EditorGUILayout.HelpBox("Select transforms that should be adjusted to fit within the cutout area (for example logos, title text, or other overlays). This effect is done by temporarily parenting the rect transform within the cutout, so only the top level rect transform is needed.", MessageType.Info);
                }

                CutoutScript cutoutScript = ((CaptureScript)target).cutoutScript;
                if (cutoutScript != null && ((RectTransform)cutoutScript.transform).hasChanged)
                    localEditorUpdateQueue.Enqueue(((CaptureScript)target).CutoutValueChanged);
            }
        }

        protected void MouseSettings()
        {
            bool showMouseSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showMouseSettings", "Mouse Settings");
            if (showMouseSettings && !((CaptureScript)target).editorWindowMode)
            {
                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showOriginalMouse"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showInGameMouse"));
                if (serializedObject.FindProperty("showInGameMouse").boolValue)
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("mouseFollowScript"));
                if (EditorGUI.EndChangeCheck())
                    localEditorUpdateQueue.Enqueue(((CaptureScript)target).UpdateMouse);
            }
        }

        protected void DontDestroyOnLoadSettings()
        {
            bool showDontDestroyOnLoadSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showDontDestroyOnLoadSettings", "DontDestroyOnLoad Settings");
            if (showDontDestroyOnLoadSettings && !((CaptureScript)target).editorWindowMode)
            {
                bool originalGuiEnabled = GUI.enabled;
                if (Application.isPlaying && serializedObject.FindProperty("dontDestroyOnLoad").boolValue)
                    GUI.enabled = false;

                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("dontDestroyOnLoad"), new GUIContent("DontDestroyOnLoad"));
                if (EditorGUI.EndChangeCheck())
                    localEditorUpdateQueue.Enqueue(((CaptureScript)target).UpdateDontDestroyOnLoad);
                GUI.enabled = originalGuiEnabled;

                if (serializedObject.FindProperty("dontDestroyOnLoad").boolValue)
                {
                    int oldValue = ((CaptureScript)target).GetMaxInstances();
                    int newValue = EditorGUILayout.IntField("Max Instances", oldValue);
                    if (newValue != oldValue)
                    {
                        ((CaptureScript)target).SetMaxInstances(newValue);
                        EditorUtility.SetDirty(target);
                    }

                    string fullMaxInstancesText = "Returning to the initial scene that creates this gameobject would normally cause multiple tools to be created. This avoids that by limiting the max number of instances available. Likely you only want one instance of the tool.";
                    fullMaxInstancesText += "\n\nIf you do have multiple instances in the scene, be sure to give them different hotkeys to avoid issues with multiple tools capturing at the same time.";
                    EditorGUILayout.HelpBox(fullMaxInstancesText, MessageType.Info);

                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("autoUpdateCameras"));
                    if (serializedObject.FindProperty("autoUpdateCameras").boolValue)
                    {
                        string buttonName = serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue ? "Find Cameras by Tag" : "Find Cameras by Name";
                        if (GUILayout.Button(buttonName))
                        {
                            serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue = !serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue;
                            localEditorUpdateQueue.Enqueue(((CaptureScript)target).AnyCameraChanged);
                        }
                    }

                    EditorGUILayout.HelpBox("Automatically finds the replacement cameras after a scene transition. To identify the replacement camera, select the proper option and have the corresponding cameras share the same unique name or tag.", MessageType.Info);
                }
            }
        }

        protected void SupportButtons()
        {
            bool showSupportSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSupportSettings", "Support");
            if (showSupportSettings)
            {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
                bool originalGUIEnabled = GUI.enabled;
                GUI.enabled &= errorLogs.Count > 0;
                if (GUILayout.Button("Upload Editor Error Logs", GUILayout.MinHeight(25)))
                {
                    serializedObject.FindProperty("uploadingEditorLogs").boolValue = true;
                    APIShare.UploadToFileIO(string.Join("\n", errorLogs.ToArray()), "1w", (resultUrl) =>
                    {
                        serializedObject.FindProperty("editorLogsUrl").stringValue = resultUrl;
                        serializedObject.FindProperty("uploadingEditorLogs").boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                GUI.enabled = originalGUIEnabled;

                bool uploadingEditorLogs = serializedObject.FindProperty("uploadingEditorLogs").boolValue;
                string editorLogsUrl = serializedObject.FindProperty("editorLogsUrl").stringValue;

                if (uploadingEditorLogs || !string.IsNullOrEmpty(editorLogsUrl))
                    EditorGUILayout.LabelField("Editor Logs Upload Url: " + (uploadingEditorLogs ? "Uploading..." : editorLogsUrl));
#endif
                if (GUILayout.Button("Email", GUILayout.MinHeight(25)))
                {
                    string subject = "Support";
                    if (ToolInfo.isScreenshotTool && ToolInfo.isGifTool)
                        subject = "Ultimate Gif & Screenshot Tool Support";
                    else if (ToolInfo.isGifTool)
                        subject = "Ultimate Gif Tool Support";
                    else if (ToolInfo.isScreenshotTool)
                        subject = "Ultimate Screenshot Tool Support";
                    string body = "Hey Jacob,\n\n\n" + DebugInformation(true, editorLogsUrl);

                    WebShare.ShareByEmail("", body, "jacob@tangledrealitystudios.com", subject);
                }

                if (GUILayout.Button("Twitter", GUILayout.MinHeight(25)))
                    WebShare.ShareToTwitter(editorLogsUrl, "@tangled_reality " + DebugInformation(false));
                if (GUILayout.Button("Discord", GUILayout.MinHeight(25)))
                    Application.OpenURL("https://discord.gg/nFuptUZ");
                if (GUILayout.Button("Reddit", GUILayout.MinHeight(25)))
                    WebShare.ShareToReddit("tangledreality", editorLogsUrl);
                EditorGUILayout.HelpBox("If you have any questions, comments, or requests, please feel free to reach out. I try to respond within a couple days.", MessageType.Info);

                if (GUILayout.Button("Rate", GUILayout.MinHeight(25)))
                {
                    if (ToolInfo.isScreenshotTool && ToolInfo.isGifTool)
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/123463");
                    else if (ToolInfo.isGifTool)
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/123460");
                    else if (ToolInfo.isScreenshotTool)
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/119675");
                }
                if (GUILayout.Button("Donate", GUILayout.MinHeight(25)))
                    Application.OpenURL("https://www.paypal.com/donate/?token=SWSWJKlDjPzKBTe55JGLGtdOchLRHAbJprXXhxpLA_HVRJUPmkoaJdk0NqIy5XeqDod9h0&country.x=US&locale.x=US");
                if (GUILayout.Button("Other Assets", GUILayout.MinHeight(25)))
                    Application.OpenURL("https://assetstore.unity.com/publishers/35845");
                EditorGUILayout.HelpBox("If you're happy with this asset, please give it a heart and 5 stars! You can also check out my other assets.", MessageType.Info);
            }
        }

        string DebugInformation(bool longform, string editorLogsUrl = "")
        {
            string os = SystemInfo.operatingSystem;
            if (!longform)
                os = os.Substring(0, os.IndexOf(" ", System.StringComparison.Ordinal));

            string debugInfo = "";
            if (longform)
            {
                debugInfo += "Debug Information:";
                if (!string.IsNullOrEmpty(editorLogsUrl))
                    debugInfo += editorLogsUrl;
                debugInfo += "\nModel: " + SystemInfo.deviceModel;
                debugInfo += "\nOS: ";
            }
            debugInfo += os;
            debugInfo += (longform ? "\n" : " ") + "Unity: " + Application.unityVersion;

            if (ToolInfo.isGifTool)
            {
                System.Type gifClass = System.Reflection.Assembly.GetAssembly(typeof(CaptureScript)).GetType("TRS.CaptureTool.GifScript");
                string version = gifClass.GetField("version").GetValue(null) as string;
                debugInfo += (longform ? "\n" : " ") + "UGTv" + version;
            }
            if (ToolInfo.isScreenshotTool)
            {
                System.Type screenshotClass = System.Reflection.Assembly.GetAssembly(typeof(CaptureScript)).GetType("TRS.CaptureTool.ScreenshotScript");
                string version = screenshotClass.GetField("version").GetValue(null) as string;
                debugInfo += (longform ? "\n" : " ") + "USTv" + version;
            }
            if (!longform && !string.IsNullOrEmpty(editorLogsUrl))
                debugInfo += editorLogsUrl;
            return debugInfo;
        }

        public static System.Type FindType(string qualifiedTypeName)
        {
            System.Type t = System.Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (System.Reflection.Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }

        void SetEditorsLogUrl(string newEditorsLogUrl)
        {
            serializedObject.FindProperty("editorLogsUrl").stringValue = newEditorsLogUrl;
            serializedObject.FindProperty("uploadingEditorLogs").boolValue = false;
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Helpers
        protected virtual void GeneralUpdates()
        {

        }

        protected virtual void UpdateSubEditors()
        {
            ShareScriptEditor oldShareScriptEditor = shareScriptEditor;
            if (((CaptureScript)target).weakShareScript != null && ((CaptureScript)target).weakShareScript.IsAlive)
            {
                if (shareScriptEditor == null)
                    shareScriptEditor = (ShareScriptEditor)Editor.CreateEditor(((CaptureScript)target).weakShareScript.Target as ShareScript);
            }
            else
                shareScriptEditor = null;

            if (tabNames == null || shareScriptEditor != oldShareScriptEditor)
                UpdateTabs();
        }

        protected virtual void UpdateTabs()
        {
            int numTabs = (int)Tab.Size;
            bool includeShare = shareScriptEditor != null;
            if (!includeShare)
                --numTabs;

            int arrayIndex = 0;
            tabNames = new string[numTabs];
            tabAtIndex = new Tab[numTabs];
            for (int i = 0; i < (int)Tab.Size; ++i)
            {
                Tab tab = (Tab)i;
                if (tab != Tab.Share || includeShare)
                {
                    tabNames[arrayIndex] = tab.ToString();
                    if (tab == Tab.Save && allowsEdit)
                        tabNames[arrayIndex] = "Edit & Save";
                    tabAtIndex[arrayIndex] = tab;
                    ++arrayIndex;
                }
            }
        }
        #endregion
    }
}
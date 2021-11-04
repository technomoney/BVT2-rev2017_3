using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ScreenshotBurstScript))]
    public class ScreenshotBurstScriptEditor : ScreenshotSubComponentScriptEditor
    {
        public override void Display()
        {
            Settings();

            Buttons();
        }

        public void Settings()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            string settingsName = ((ScreenshotBurstScript)target).subWindowMode ? "Burst Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((ScreenshotBurstScript)target).editorWindowMode || ((ScreenshotBurstScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);
                }

                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showTakeAllScreenshotBurstButton"), new GUIContent("Show Take All Button"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("takeScreenshotBurstKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("burstSize"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("skipFrames"));

                int burstSize = serializedObject.FindProperty("burstSize").intValue;
                if (burstSize <= 0)
                {
                    burstSize = 1;
                    serializedObject.FindProperty("burstSize").intValue = burstSize;
                }

                int skipFrames = serializedObject.FindProperty("skipFrames").intValue;
                if (skipFrames < 0)
                {
                    skipFrames = 0;
                    serializedObject.FindProperty("skipFrames").intValue = skipFrames;
                }

                string helpString = "Take burst of " + burstSize + " screenshots.";
                if (skipFrames > 0)
                {
                    if (skipFrames == 1)
                        helpString += " Capturing every other frame.";
                    else
                        helpString += " Capturing every " + skipFrames + " frames.";
                }

                EditorGUILayout.HelpBox(helpString, MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public void Buttons()
        {
            TakeBurstButton();
            TakeAllBurstButton();
        }

        public void TakeBurstButton()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            if (GUILayout.Button("Take Screenshot Burst", GUILayout.MinHeight(40)))
                ((ScreenshotBurstScript)target).TakeScreenshotBurst();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeAllBurstButton()
        {
            if (!serializedObject.FindProperty("showTakeAllScreenshotBurstButton").boolValue)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            if (GUILayout.Button("Take All Screenshots Burst", GUILayout.MinHeight(40)))
                ((ScreenshotBurstScript)target).TakeAllScreenshotBurst();
            GUI.enabled = originalGUIEnabled;
        }
    }
}
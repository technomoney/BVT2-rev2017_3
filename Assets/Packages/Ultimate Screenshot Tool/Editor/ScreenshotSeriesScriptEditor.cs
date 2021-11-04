using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ScreenshotSeriesScript))]
    public class ScreenshotSeriesScriptEditor : ScreenshotSubComponentScriptEditor
    {
        ReorderableList buttonInteractionList;

        void OnEnable()
        {
            if (target == null)
                return;

            buttonInteractionList = new ReorderableList(serializedObject,
                                                 serializedObject.FindProperty("buttonInteractions"),
                                 true, true, true, true);

            string[] headerTexts = { "Button to Press", "Animation Delay", "Take Photo" };
            string[] properties = { "button", "animationDelay", "takePhoto" };
            float[] widths = { 0.4f, 0.4f, 0.2f };

            buttonInteractionList.AddHeader(headerTexts, widths);
            buttonInteractionList.AddStandardElementCallback(properties, widths);
        }


        public override void Display()
        {
            Settings();

            Button();
        }

        public void Settings()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            string settingsName = ((ScreenshotSeriesScript)target).subWindowMode ? "Screenshot Series Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((ScreenshotSeriesScript)target).editorWindowMode || ((ScreenshotSeriesScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Capture Initial Screen");
                serializedObject.FindProperty("captureInitialScreen").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("captureInitialScreen").boolValue);
                EditorGUILayout.EndHorizontal();

                buttonInteractionList.DoLayoutList();

                if (serializedObject.FindProperty("buttonInteractions").arraySize == 0)
                    EditorGUILayout.HelpBox("Entering rows without a a button press to create a delay is valid too. (You could even use this as a macro to jump around your app.)", MessageType.Info);

                //    if (!Application.isPlaying)
                //      EditorGUILayout.HelpBox("ScreenshotSeriesScript will not work properly while game is not playing. (Delays won't work.) Press play to enable button.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public void Button()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            if (GUILayout.Button("Take Screenshot Series", GUILayout.MinHeight(40)))
                ((ScreenshotSeriesScript)target).TakeScreenshotSeries();
            GUI.enabled = originalGUIEnabled;
        }
    }
}
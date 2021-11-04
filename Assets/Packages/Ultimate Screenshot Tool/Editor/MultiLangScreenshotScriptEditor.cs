using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(MultiLangScreenshotScript))]
    public class MultiLangScreenshotScriptEditor : ScreenshotSubComponentScriptEditor
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

            string settingsName = ((MultiLangScreenshotScript)target).subWindowMode ? "Multi-Language Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((MultiLangScreenshotScript)target).editorWindowMode || ((MultiLangScreenshotScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);

                    ScreenshotSeriesScript currentScreenshotSeriesScript = (ScreenshotSeriesScript)serializedObject.FindProperty("screenshotSeriesScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotSeriesScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Series Script", currentScreenshotSeriesScript, typeof(ScreenshotSeriesScript), true);
                }

                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("languages"), true);
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public void Buttons()
        {
            TakeSeriesButton();
            TakeAllButton();
        }

        public void TakeSeriesButton()
        {
            ScreenshotSeriesScript currentScreenshotSeriesScript = (ScreenshotSeriesScript)serializedObject.FindProperty("screenshotSeriesScript").objectReferenceValue;
            if (currentScreenshotSeriesScript == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            if (GUILayout.Button("Take Multi-Language Screenshot Series", GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeMultiLangScreenshotSeries();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeAllButton()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            if (GUILayout.Button("Take All Multi-Language Screenshots", GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeAllMultiLangScreenshots();
            GUI.enabled = originalGUIEnabled;
        }
    }
}
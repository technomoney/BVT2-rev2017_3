using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    public class ScreenshotSubComponentScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (target == null || ((ScreenshotSubComponentScript)target).hiddenMode)
                return;

            if (!(((ScreenshotSubComponentScript)target).editorWindowMode || ((ScreenshotSubComponentScript)target).subWindowMode))
                EditorGUILayout.Space();

            Display();
        }

        public virtual void Display()
        {

        }
    }
}
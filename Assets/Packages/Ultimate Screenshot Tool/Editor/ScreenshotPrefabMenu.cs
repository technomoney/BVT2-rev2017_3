using UnityEngine;
using UnityEditor;

// Add hotkeys:
// https://unity3d.com/learn/tutorials/topics/interface-essentials/unity-editor-extensions-menu-items

namespace TRS.CaptureTool
{
    public class ScreenshotPrefabMenu : EditorWindow
    {
        readonly static string SCREENSHOT_PREFAB_NAME = "ScreenshotTool";

        static GameObject screenshotTool;

        [MenuItem("Tools/Ultimate Screenshot Tool/Create Screenshot Prefab", false, 11)]
        static void CreateScreenshotPrefab()
        {
            string[] screenshotToolGuids = AssetDatabase.FindAssets(SCREENSHOT_PREFAB_NAME + " t:GameObject", null);
            if (screenshotToolGuids.Length != 1)
            {
                Debug.LogError("Screenshot prefab not found. You may have changed the prefab name or have a duplicate. Please update Ultimate Screenshot Tool/Editor/ScreenshotPrefabMenu.cs.");
                return;
            }

            GameObject screenshotToolPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(screenshotToolGuids[0]), typeof(GameObject)) as GameObject;
            screenshotTool = Instantiate(screenshotToolPrefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(screenshotTool, "Created Screenshot Tool");
        }

        [MenuItem("Tools/Ultimate Screenshot Tool/Select Screenshot Prefab", false, 12)]
        static void SelectScreenshotPrefab()
        {
            if (screenshotTool == null)
            {
                ScreenshotScript screenshotScript = GameObject.FindObjectOfType<ScreenshotScript>() as ScreenshotScript;
                if (screenshotScript != null)
                    screenshotTool = screenshotScript.gameObject;
            }

            if (screenshotTool != null)
                Selection.activeGameObject = screenshotTool;
            else
                Debug.LogError("No screenshot tool in scene.");
        }

        [MenuItem("Tools/Ultimate Screenshot Tool/Destroy Screenshot Prefab(s)", false, 13)]
        static void DestroyScreenshotPrefabs()
        {
            ScreenshotScript[] screenshotScripts = GameObject.FindObjectsOfType<ScreenshotScript>() as ScreenshotScript[];
            foreach (ScreenshotScript screenshotScript in screenshotScripts)
                Undo.DestroyObjectImmediate(screenshotScript.gameObject);
        }
    }
}
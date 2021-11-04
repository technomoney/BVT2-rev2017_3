using UnityEngine;
using UnityEditor;
using HeathenEngineering.UIX;
public class UIXKeyboardMenuItems
{
    [MenuItem("GameObject/UIX/Composite Controls/UIX Keyboard", false, 43116)]
    static void CreateKeyboard(MenuCommand menuCommand)
    {
        GameObject go = new GameObject(GameObjectUtility.GetUniqueNameForSibling((menuCommand.context as GameObject).transform, "VirtualKeyboard"));
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        var rt = go.AddComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 489);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 209);
        go.AddComponent<Keyboard>();
        go.AddComponent<KeyboardOutputManager>();
        go.AddComponent<KeyboardTemplateManager>();
    }
}
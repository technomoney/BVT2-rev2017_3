using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using HeathenEngineering.UIX;

[CustomEditor(typeof(HeathenEngineering.UIX.KeyboardOutputManager))]
public class HeathenKeyboardOutputManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        HeathenEngineering.UIX.KeyboardOutputManager keyboard = target as HeathenEngineering.UIX.KeyboardOutputManager;
        
        //keyboard.insertPoint = EditorGUILayout.IntField("Insert Point", keyboard.insertPoint);
        //keyboard.selectionLength = EditorGUILayout.IntField("Selection Width", keyboard.selectionLength);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        DoOutputLink(keyboard);

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    void DoOutputLink(HeathenEngineering.UIX.KeyboardOutputManager keyboard)
    {
        keyboard.autoLinkHID = EditorGUILayout.ToggleLeft("Respond to 'real' keyboard input", keyboard.autoLinkHID);

        keyboard.targetType = (KeyboardOutputTargetType)EditorGUILayout.EnumPopup("Target Type", keyboard.targetType);

        //keyboard.autoTargetInputfields = EditorGUILayout.ToggleLeft("Auto target input fields", keyboard.autoTargetInputfields);

        //if (keyboard.insertPoint < 0)
        //{
        //    if (EditorGUILayout.ToggleLeft("Track input field insert point", false))
        //    {
        //        keyboard.insertPoint = 0;
        //    }
        //}
        //else
        //{
        //    if (!EditorGUILayout.ToggleLeft("Track input field insert point", true))
        //    {
        //        keyboard.insertPoint = -1;
        //    }
        //}

        if (keyboard.targetType == KeyboardOutputTargetType.Component)
        {
            keyboard.linkedGameObject = EditorGUILayout.ObjectField("Linked GameObject", keyboard.linkedGameObject, typeof(GameObject), true) as GameObject;

            if (keyboard.linkedGameObject != null)
            {
                keyboard.ValidateLinkedData();
                List<string> options = new List<string>();
                foreach (Component com in keyboard.linkedBehaviours)
                {
                    options.Add(com.GetType().ToString());

                }
                int indexOf = keyboard.linkedBehaviours.IndexOf(keyboard.linkedBehaviour);
                int newIndex = EditorGUILayout.Popup("On Behaviour", indexOf, options.ToArray());
                if (indexOf != newIndex)
                {
                    keyboard.linkedBehaviour = keyboard.linkedBehaviours[newIndex];
                    keyboard.ValidateLinkedData();
                    if (keyboard.fields.Count <= 0)
                        return;
                }
                //Debug.Log("Found properties to list");
                indexOf = keyboard.fields.IndexOf(keyboard.field);
                newIndex = EditorGUILayout.Popup("For Property", indexOf, keyboard.fields.ToArray());
                if (newIndex != indexOf)
                {
                    keyboard.field = keyboard.fields[newIndex];
                    EditorUtility.SetDirty(target);
                }
            }
        }
        else if (keyboard.targetType == KeyboardOutputTargetType.InputField)
        {
            keyboard.ManualSetInputTarget(EditorGUILayout.ObjectField("Target", keyboard.lastInputField, typeof(UnityEngine.UI.InputField), true) as UnityEngine.UI.InputField);
        }
        else if (keyboard.targetType == KeyboardOutputTargetType.Text)
        {
            if (keyboard.linkedBehaviour != null && keyboard.linkedBehaviour.GetComponent<UnityEngine.UI.Text>() == null)
                keyboard.ManualSetTextTarget(null);

            keyboard.ManualSetTextTarget(EditorGUILayout.ObjectField("Target", keyboard.linkedBehaviour, typeof(UnityEngine.UI.Text), true) as UnityEngine.UI.Text);
        }
        else if (keyboard.targetType == KeyboardOutputTargetType.Function)
        {
            SerializedProperty eventProp = serializedObject.FindProperty("keyStrokeEvent");
            
            EditorGUILayout.PropertyField(eventProp);
        }
    }

    //void ShowRows(HeathenEngineering.UIX.Keyboard keyboard)
    //{
    //    keyboard.headerRowTransform = EditorGUILayout.ObjectField("Header Row", keyboard.headerRowTransform, typeof(RectTransform), true) as RectTransform;
    //    if (keyboard.rowTransforms == null)
    //        keyboard.rowTransforms = new System.Collections.Generic.List<RectTransform>();

    //    int rowCount = EditorGUILayout.IntField("Rows", keyboard.rowTransforms.Count);

    //    if (rowCount > keyboard.rowTransforms.Count)
    //    {
    //        int i = 1;
    //        foreach (RectTransform row in keyboard.rowTransforms)
    //        {
    //            keyboard.rowTransforms[i - 1] = EditorGUILayout.ObjectField("Row " + i.ToString(), row, typeof(RectTransform), true) as RectTransform;
    //            i++;
    //        }
    //        for (int x = 0; x < rowCount - (i - 1); x++)
    //        {
    //            keyboard.rowTransforms.Add(null);
    //        }
    //    }
    //    else if (rowCount < keyboard.rowTransforms.Count)
    //    {
    //        List<RectTransform> nTrans = new List<RectTransform>();
    //        for (int i = 0; i < rowCount; i++)
    //        {
    //            nTrans.Add(keyboard.rowTransforms[i]);
    //        }
    //        keyboard.rowTransforms.Clear();
    //        keyboard.rowTransforms.AddRange(nTrans);
    //    }
    //    else
    //    {
    //        int i = 1;
    //        foreach (RectTransform row in keyboard.rowTransforms)
    //        {
    //            keyboard.rowTransforms[i - 1] = EditorGUILayout.ObjectField("Row " + i.ToString(), row, typeof(RectTransform), true) as RectTransform;
    //            i++;
    //        }
    //    }
    //}

    //void SaveToTemplate(HeathenEngineering.UIX.Keyboard keyboard)
    //{
    //    //TODO: save the current template to disk
    //}

    //void LoadTemplate(HeathenEngineering.UIX.Keyboard keyboard)
    //{
    //    TextAsset newAsset = EditorGUILayout.ObjectField("Load", null, typeof(TextAsset), false) as TextAsset;
    //    if (newAsset != null)
    //    {
    //        XmlSerializer deserializer = new XmlSerializer(typeof(HeathenEngineering.UIX.Serialization.KeyboardTemplate));
    //        StringReader reader = new StringReader(newAsset.text);

    //        try
    //        {
    //            HeathenEngineering.UIX.Serialization.KeyboardTemplate result = deserializer.Deserialize(reader) as HeathenEngineering.UIX.Serialization.KeyboardTemplate;
    //            reader.Close();

    //            if (result != null)
    //            {
    //                keyboard.selectedTemplate = result;
    //                ReloadFromTemplate(keyboard);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError("Failed to read the selected template. Message: " + ex.Message);
    //        }
    //    }
    //}

    //void ReloadFromTemplate(HeathenEngineering.UIX.Keyboard keyboard)
    //{
    //    try
    //    {
    //        if (keyboard.selectedTemplate != null)
    //        {
    //            keyboard.workingTemplate = keyboard.selectedTemplate;
    //            keyboard.RefreshKeyboard();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError("Failed to load the selected template. Message: " + ex.Message);
    //    }
    //}

    //bool drawButton(string label, float width)
    //{
    //    Rect r = EditorGUILayout.BeginHorizontal("Button", GUILayout.Width(width));
    //    if (GUI.Button(r, GUIContent.none))
    //        return true;
    //    GUILayout.Label(label);
    //    EditorGUILayout.EndHorizontal();
    //    return false;
    //}

    //bool drawButton(string label)
    //{
    //    Rect r = EditorGUILayout.BeginHorizontal("Button");
    //    if (GUI.Button(r, GUIContent.none))
    //        return true;
    //    GUILayout.Label(label);
    //    EditorGUILayout.EndHorizontal();
    //    return false;
    //}

}

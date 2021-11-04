﻿using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Collections.Generic;
[CustomEditor(typeof(HeathenEngineering.UIX.KeyboardTemplateManager))]
public class HeathenKeyboardTemplateManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        HeathenEngineering.UIX.KeyboardTemplateManager keyboard = target as HeathenEngineering.UIX.KeyboardTemplateManager;

        if (keyboard.workingTemplate == null)
            keyboard.workingTemplate = new HeathenEngineering.UIX.Serialization.KeyboardTemplate() { TemplateName = "New Template" };

        keyboard.RefreshTemplate();

        EditorGUILayout.LabelField("Template", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (drawButton("Save As", 45))
        {
            keyboard.RefreshTemplate();
            string pathTarget = EditorUtility.SaveFilePanel("Save Keyboard Template", Application.dataPath, keyboard.workingTemplate.TemplateName, "xml");
            if (!string.IsNullOrEmpty(pathTarget))
            {
                try
                {
                    XmlSerializer serialize = new XmlSerializer(typeof(HeathenEngineering.UIX.Serialization.KeyboardTemplate));
                    StreamWriter fileStream = new StreamWriter(pathTarget);
                    serialize.Serialize(fileStream, keyboard.workingTemplate);
                    fileStream.Close();
                    fileStream.Dispose();
                }
                catch
                {
                    Debug.LogError("An error occured while attempting to save the keyboard's template data");
                }
            }
        }
        if (keyboard.selectedTemplate != null)
        {
            if (drawButton("Refresh From", 45))
            {
                ReloadFromTemplate(keyboard);
            }
        }
        EditorGUILayout.EndHorizontal();
        LoadTemplate(keyboard);
        EditorGUILayout.LabelField("Current: " + (keyboard.selectedTemplate == null || string.IsNullOrEmpty(keyboard.selectedTemplate.TemplateName) ? "Unnamed" : keyboard.selectedTemplate.TemplateName));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        keyboard.Prototype = EditorGUILayout.ObjectField("Prototype", keyboard.Prototype, typeof(HeathenEngineering.UIX.KeyboardKey), false) as HeathenEngineering.UIX.KeyboardKey;
        keyboard.Container = EditorGUILayout.ObjectField("Container", keyboard.Container, typeof(RectTransform), true) as RectTransform;
        //keyboard.insertPoint = EditorGUILayout.IntField("Insert Point", keyboard.insertPoint);
        //keyboard.selectionLength = EditorGUILayout.IntField("Selection Width", keyboard.selectionLength);
        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
        //keyboard.useShiftToggle = EditorGUILayout.ToggleLeft("Shift is a toggle", keyboard.useShiftToggle);
        //keyboard.useAltGrToggle = EditorGUILayout.ToggleLeft("Alt Gr is a toggle", keyboard.useAltGrToggle);
        EditorGUILayout.Space();
        //EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        //DoOutputLink(keyboard);
        //EditorGUILayout.Space();
        if (keyboard.headerRowTransform != null && keyboard.Container != null && keyboard.Prototype != null)
        {
            EditorGUILayout.HelpBox("If you do not intend to load templates at run time you can safely remove the Template Manager without effecting the instantiated keyboard.", MessageType.Info);
            EditorGUILayout.LabelField("Transforms", EditorStyles.boldLabel);
            ShowRows(keyboard);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    //void DoOutputLink(HeathenEngineering.UIX.KeyboardTemplateManager keyboard)
    //{
    //    keyboard.autoLinkHID = EditorGUILayout.ToggleLeft("Respond to keyboard input", keyboard.autoLinkHID);
    //    keyboard.autoTargetInputfields = EditorGUILayout.ToggleLeft("Auto target input fields", keyboard.autoTargetInputfields);

    //    if (keyboard.insertPoint < 0)
    //    {
    //        if (EditorGUILayout.ToggleLeft("Track input field insert point", false))
    //        {
    //            keyboard.insertPoint = 0;
    //        }
    //    }
    //    else
    //    {
    //        if (!EditorGUILayout.ToggleLeft("Track input field insert point", true))
    //        {
    //            keyboard.insertPoint = -1;
    //        }
    //    }

    //    if (!keyboard.autoTargetInputfields)
    //    {
    //        keyboard.linkedGameObject = EditorGUILayout.ObjectField("Linked GameObject", keyboard.linkedGameObject, typeof(GameObject), true) as GameObject;

    //        if (keyboard.linkedGameObject != null)
    //        {
    //            keyboard.ValidateLinkedData();
    //            List<string> options = new List<string>();
    //            foreach (Component com in keyboard.linkedBehaviours)
    //            {
    //                options.Add(com.GetType().ToString());

    //            }
    //            int indexOf = keyboard.linkedBehaviours.IndexOf(keyboard.linkedBehaviour);
    //            int newIndex = EditorGUILayout.Popup("On Behaviour", indexOf, options.ToArray());
    //            if (indexOf != newIndex)
    //            {
    //                keyboard.linkedBehaviour = keyboard.linkedBehaviours[newIndex];
    //                keyboard.ValidateLinkedData();
    //                if (keyboard.fields.Count <= 0)
    //                    return;
    //            }
    //            //Debug.Log("Found properties to list");
    //            indexOf = keyboard.fields.IndexOf(keyboard.field);
    //            newIndex = EditorGUILayout.Popup("For Property", indexOf, keyboard.fields.ToArray());
    //            if (newIndex != indexOf)
    //            {
    //                keyboard.field = keyboard.fields[newIndex];
    //                EditorUtility.SetDirty(target);
    //            }
    //        }
    //    }
    //}

    void ShowRows(HeathenEngineering.UIX.KeyboardTemplateManager keyboard)
    {
        keyboard.headerRowTransform = EditorGUILayout.ObjectField("Header Row", keyboard.headerRowTransform, typeof(RectTransform), true) as RectTransform;
        if (keyboard.rowTransforms == null)
            keyboard.rowTransforms = new System.Collections.Generic.List<RectTransform>();

        int rowCount = EditorGUILayout.IntField("Rows", keyboard.rowTransforms.Count);

        if (rowCount > keyboard.rowTransforms.Count)
        {
            int i = 1;
            foreach (RectTransform row in keyboard.rowTransforms)
            {
                keyboard.rowTransforms[i - 1] = EditorGUILayout.ObjectField("Row " + i.ToString(), row, typeof(RectTransform), true) as RectTransform;
                i++;
            }
            for (int x = 0; x < rowCount - (i - 1); x++)
            {
                keyboard.rowTransforms.Add(null);
            }
        }
        else if (rowCount < keyboard.rowTransforms.Count)
        {
            List<RectTransform> nTrans = new List<RectTransform>();
            for (int i = 0; i < rowCount; i++)
            {
                nTrans.Add(keyboard.rowTransforms[i]);
            }
            keyboard.rowTransforms.Clear();
            keyboard.rowTransforms.AddRange(nTrans);
        }
        else
        {
            int i = 1;
            foreach (RectTransform row in keyboard.rowTransforms)
            {
                keyboard.rowTransforms[i - 1] = EditorGUILayout.ObjectField("Row " + i.ToString(), row, typeof(RectTransform), true) as RectTransform;
                i++;
            }
        }
    }

    //void SaveToTemplate(HeathenEngineering.UIX.KeyboardTemplateManager keyboard)
    //{
    //    //TODO: save the current template to disk
    //}

    void LoadTemplate(HeathenEngineering.UIX.KeyboardTemplateManager keyboard)
    {
        if (keyboard.Prototype != null && keyboard.Container != null)
        {
            TextAsset newAsset = EditorGUILayout.ObjectField("Load", null, typeof(TextAsset), false) as TextAsset;
            if (newAsset != null)
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(HeathenEngineering.UIX.Serialization.KeyboardTemplate));
                StringReader reader = new StringReader(newAsset.text);

                try
                {
                    HeathenEngineering.UIX.Serialization.KeyboardTemplate result = deserializer.Deserialize(reader) as HeathenEngineering.UIX.Serialization.KeyboardTemplate;
                    reader.Close();

                    if (result != null)
                    {
                        keyboard.selectedTemplate = result;
                        ReloadFromTemplate(keyboard);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to read the selected template. Message: " + ex.Message);
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("You must provide a key prototype in the Prototype field and a target Rect Transform in the Container field in order to load a template.", MessageType.Info);
        }
    }

    void ReloadFromTemplate(HeathenEngineering.UIX.KeyboardTemplateManager keyboard)
    {
        try
        {
            if (keyboard.selectedTemplate != null)
            {
                keyboard.workingTemplate = keyboard.selectedTemplate;
                keyboard.RefreshKeyboard();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load the selected template. Message: " + ex.Message);
        }
    }

    bool drawButton(string label, float width)
    {
        Rect r = EditorGUILayout.BeginHorizontal("Button", GUILayout.Width(width));
        if (GUI.Button(r, GUIContent.none))
            return true;
        GUILayout.Label(label);
        EditorGUILayout.EndHorizontal();
        return false;
    }

    bool drawButton(string label)
    {
        Rect r = EditorGUILayout.BeginHorizontal("Button");
        if (GUI.Button(r, GUIContent.none))
            return true;
        GUILayout.Label(label);
        EditorGUILayout.EndHorizontal();
        return false;
    }

}

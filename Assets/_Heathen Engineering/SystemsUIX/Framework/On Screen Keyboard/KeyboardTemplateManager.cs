﻿using UnityEngine;
using System.Collections.Generic;
using HeathenEngineering.UIX.Serialization;

namespace HeathenEngineering.UIX
{
    [AddComponentMenu("Heathen/UIX/Keyboard/Keyboard Template Manager")]
    [RequireComponent(typeof(Keyboard))]
    public class KeyboardTemplateManager : MonoBehaviour
    {
        [Header("References")]
        public KeyboardKey Prototype;
        public RectTransform Container;
        public Serialization.KeyboardTemplate workingTemplate;
        public Serialization.KeyboardTemplate selectedTemplate;
        [Header("Transforms")]
        public RectTransform headerRowTransform;
        public List<RectTransform> rowTransforms;

        private Keyboard keyboard;

        /// <summary>
        /// Refresh the Template with the current keyboard structure
        /// </summary>
        public void RefreshTemplate()
        {
            keyboard = GetComponent<Keyboard>();

            if (keyboard == null)
                return;

            if (workingTemplate == null)
                workingTemplate = new Serialization.KeyboardTemplate() { TemplateName = "New Template" };

            if (headerRowTransform == null)
                workingTemplate.HeaderRow = null;
            else
            {
                if (workingTemplate.HeaderRow == null)
                    workingTemplate.HeaderRow = new Serialization.KeyboardTemplateRow();

                workingTemplate.HeaderRow.RowOffset = headerRowTransform.anchoredPosition3D;
                workingTemplate.HeaderRow.RowRotation = headerRowTransform.localEulerAngles;

                List<KeyboardKeyTemplate> keyTemplates = new List<KeyboardKeyTemplate>();

                foreach (Transform trans in headerRowTransform)
                {
                    KeyboardKey key = trans.gameObject.GetComponent<KeyboardKey>();
                    if (key != null)
                    {
                        key.UpdateTemplate(ref key.template);
                        keyTemplates.Add(key.template);
                    }
                }

                workingTemplate.HeaderRow.Keys = keyTemplates.ToArray();
            }

            List<KeyboardTemplateRow> rowTemplates = new List<KeyboardTemplateRow>();

            if (rowTransforms == null)
                rowTransforms = new List<RectTransform>();

            foreach (RectTransform row in rowTransforms)
            {
                if (row == null)
                    continue;

                KeyboardTemplateRow nRow = new KeyboardTemplateRow();
                nRow.RowOffset = row.anchoredPosition3D;
                nRow.RowRotation = row.localEulerAngles;

                List<KeyboardKeyTemplate> keyTemplates = new List<KeyboardKeyTemplate>();

                foreach (Transform trans in row)
                {
                    KeyboardKey key = trans.gameObject.GetComponent<KeyboardKey>();
                    if (key != null)
                    {
                        key.UpdateTemplate(ref key.template);
                        keyTemplates.Add(key.template);
                    }
                }

                nRow.Keys = keyTemplates.ToArray();
                rowTemplates.Add(nRow);
            }

            workingTemplate.PrimaryRows = rowTemplates.ToArray();
            workingTemplate.TemplateName = name;
        }

        /// <summary>
        /// Refresh the keyboard with the current templates structure
        /// </summary>
        public void RefreshKeyboard()
        {
            keyboard = GetComponent<Keyboard>();

            if (keyboard == null)
                return;

            if (Prototype == null)
            {
                Debug.LogError("An attempt to refresh keyboard '" + name + "' failed with error: No Key Prototype present unable to generate keyboard.");
                return;
            }

            if (Container == null)
            {
                Debug.LogWarning("An attempt to refresh keyboard '" + name + "' logged a warning: No Key Container present generating new transform.");
                GameObject nContainer = new GameObject("Key Container", typeof(RectTransform));
                Container = nContainer.GetComponent<RectTransform>();
                Container.SetParent(transform);
                Container.localScale = Vector3.one;
                Container.localEulerAngles = Vector3.zero;
                Container.pivot = new Vector2(0.5f, 0.5f);
                Container.anchoredPosition3D = Vector3.zero;
            }

            List<GameObject> killList = new List<GameObject>();
            for (int c = 0; c < Container.childCount; c++)
            {
                killList.Add(Container.GetChild(c).gameObject);
            }
            foreach (GameObject go in killList)
            {
                if (Application.isEditor)
                    DestroyImmediate(go);
                else
                    Destroy(go);
            }

            if (workingTemplate.HeaderRow != null)
            {
                GameObject headerRow = new GameObject("Header Row", typeof(RectTransform));
                RectTransform headerRowTrans = headerRow.GetComponent<RectTransform>();
                headerRowTransform = headerRowTrans;
                headerRowTrans.SetParent(Container);
                headerRowTrans.localScale = Vector3.one;
                headerRowTrans.pivot = new Vector2(0.5f, 0.5f);
                headerRowTrans.anchoredPosition3D = workingTemplate.HeaderRow.RowOffset;
                headerRowTrans.localEulerAngles = workingTemplate.HeaderRow.RowRotation;

                foreach (Serialization.KeyboardKeyTemplate key in workingTemplate.HeaderRow.Keys)
                {
                    GameObject newKey = Instantiate<GameObject>(Prototype.gameObject);
                    newKey.name = (key.Code != KeyCode.None ? key.Code.ToString() : key.DisplayNormal) + " key";
                    KeyboardKey newKeyboardKey = newKey.GetComponent<KeyboardKey>();

                    newKeyboardKey.pressed += keyboard.keyPressed;
                    newKeyboardKey.keyGlyph.ApplyTemplate(key);
                    if (newKeyboardKey.selfRectTransform == null)
                        newKeyboardKey.selfRectTransform = newKey.GetComponent<RectTransform>();

                    newKeyboardKey.selfRectTransform.SetParent(headerRowTrans);

                    newKeyboardKey.selfRectTransform.anchoredPosition3D = key.KeyOffset;
                    newKeyboardKey.selfRectTransform.localEulerAngles = key.KeyRotation;
                    newKeyboardKey.selfRectTransform.sizeDelta = key.KeySize;
                    newKeyboardKey.selfRectTransform.localScale = Vector3.one;

                    newKeyboardKey.keyGlyph.altGrString = key.AltGr;
                    newKeyboardKey.keyGlyph.shiftedString = key.Shifted;
                    newKeyboardKey.keyGlyph.normalString = key.Normal;
                    newKeyboardKey.keyGlyph.shiftedAltGrString = key.ShiftedAltGr;
                    newKeyboardKey.keyGlyph.code = key.Code;
                    newKeyboardKey.keyType = key.KeyType;

                    if (newKeyboardKey.keyGlyph.altGrDisplay != null)
                        newKeyboardKey.keyGlyph.altGrDisplay.text = key.DisplayAltGr;

                    if (newKeyboardKey.keyGlyph.shiftedDisplay != null)
                        newKeyboardKey.keyGlyph.shiftedDisplay.text = key.DisplayShifted;

                    if (newKeyboardKey.keyGlyph.normalDisplay != null)
                        newKeyboardKey.keyGlyph.normalDisplay.text = key.DisplayNormal;

                    if (newKeyboardKey.keyGlyph.shiftedAltGrDisplay != null)
                        newKeyboardKey.keyGlyph.shiftedAltGrDisplay.text = key.DisplayShiftedAltGr;

                }
            }

            int i = 1;
            rowTransforms.Clear();
            foreach (Serialization.KeyboardTemplateRow row in workingTemplate.PrimaryRows)
            {
                GameObject nRow = new GameObject("Row " + i.ToString(), typeof(RectTransform));
                RectTransform nRowTrans = nRow.GetComponent<RectTransform>();
                rowTransforms.Add(nRowTrans);
                nRowTrans.SetParent(Container);
                nRowTrans.pivot = new Vector2(0.5f, 0.5f);
                nRowTrans.anchoredPosition3D = row.RowOffset;
                nRowTrans.localEulerAngles = row.RowRotation;
                nRowTrans.localScale = Vector3.one;

                foreach (Serialization.KeyboardKeyTemplate key in row.Keys)
                {
                    GameObject newKey = Instantiate<GameObject>(Prototype.gameObject);
                    newKey.name = (key.Code != KeyCode.None ? key.Code.ToString() : key.DisplayNormal) + " key";
                    KeyboardKey newKeyboardKey = newKey.GetComponent<KeyboardKey>();

                    newKeyboardKey.pressed += keyboard.keyPressed;
                    newKeyboardKey.keyGlyph.ApplyTemplate(key);
                    if (newKeyboardKey.selfRectTransform == null)
                        newKeyboardKey.selfRectTransform = newKey.GetComponent<RectTransform>();

                    newKeyboardKey.selfRectTransform.SetParent(nRowTrans);

                    newKeyboardKey.selfRectTransform.anchoredPosition3D = key.KeyOffset;
                    newKeyboardKey.selfRectTransform.localEulerAngles = key.KeyRotation;
                    newKeyboardKey.selfRectTransform.sizeDelta = key.KeySize;
                    newKeyboardKey.selfRectTransform.localScale = Vector3.one;
                    newKeyboardKey.keyType = key.KeyType;

                    newKeyboardKey.keyGlyph.altGrString = key.AltGr;
                    newKeyboardKey.keyGlyph.shiftedString = key.Shifted;
                    newKeyboardKey.keyGlyph.normalString = key.Normal;
                    newKeyboardKey.keyGlyph.shiftedAltGrString = key.ShiftedAltGr;
                    newKeyboardKey.keyGlyph.code = key.Code;

                    if (newKeyboardKey.keyGlyph.altGrDisplay != null)
                        newKeyboardKey.keyGlyph.altGrDisplay.text = key.DisplayAltGr;

                    if (newKeyboardKey.keyGlyph.shiftedDisplay != null)
                        newKeyboardKey.keyGlyph.shiftedDisplay.text = key.DisplayShifted;

                    if (newKeyboardKey.keyGlyph.normalDisplay != null)
                        newKeyboardKey.keyGlyph.normalDisplay.text = key.DisplayNormal;

                    if (newKeyboardKey.keyGlyph.shiftedAltGrDisplay != null)
                        newKeyboardKey.keyGlyph.shiftedAltGrDisplay.text = key.DisplayShiftedAltGr;
                }
                i++;
            }
        }
    }
}

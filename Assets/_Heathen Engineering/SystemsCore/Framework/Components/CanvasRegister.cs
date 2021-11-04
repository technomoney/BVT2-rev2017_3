﻿using UnityEngine;

namespace HeathenEngineering.Scriptable
{
    [RequireComponent(typeof(Canvas))]
    [AddComponentMenu("Heathen/Canvas/Canvas Register")]
    public class CanvasRegister : MonoBehaviour
    {
        public CanvasVariable ReferenceVariable;

        private void Start()
        {
            if (ReferenceVariable != null)
                ReferenceVariable.canvas = GetComponent<Canvas>();
        }

        private void OnDestroy()
        {
            if (ReferenceVariable != null)
                ReferenceVariable.canvas = null;
        }
    }
}

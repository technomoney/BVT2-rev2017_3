﻿using UnityEngine;

namespace HeathenEngineering.Scriptable
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Heathen/Camera/Camera Register")]
    public class CameraRegister : MonoBehaviour
    {
        public CameraVariable ReferenceVariable;

        private void Start()
        {
            if (ReferenceVariable != null)
                ReferenceVariable.camera = GetComponent<Camera>();
        }

        private void OnDestroy()
        {
            if (ReferenceVariable != null)
                ReferenceVariable.camera = null;
        }
    }
}

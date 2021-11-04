﻿using UnityEngine;

namespace HeathenEngineering.Scriptable
{
    [AddComponentMenu("Heathen/Generic/Game Object Register")]
    public class GameObjectRegister : MonoBehaviour
    {
        public GameObjectVariable ReferenceVariable;

        private void Start()
        {
            if (ReferenceVariable != null)
                ReferenceVariable.gameObject = gameObject;
        }
        
        private void OnDestroy()
        {
            if (ReferenceVariable != null)
                ReferenceVariable.gameObject = null;
        }
    }
}

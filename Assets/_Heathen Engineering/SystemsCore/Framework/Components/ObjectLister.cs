﻿using System;
using UnityEngine;

namespace HeathenEngineering.Scriptable
{
    [AddComponentMenu("Heathen/Generic/Game Object Lister")]
    public class ObjectLister : MonoBehaviour
    {
        public ObjectList TargetList;

        private void OnEnable()
        {
            if (TargetList != null)
                TargetList.AddObject(gameObject);
        }

        private void OnDisable()
        {
            if (TargetList != null)
                TargetList.RemoveObject(gameObject);
        }
    }
}

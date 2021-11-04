﻿using HeathenEngineering.Serializable;
using UnityEngine;

namespace HeathenEngineering.Scriptable
{
    [AddComponentMenu("Heathen/Events/Collision Stay Sender")]
    public class CollisionStaySender : MonoBehaviour
    {
        [Header("Game Event")]
        public CollisionGameEvent PhysicsEvent;

        [Header("Direct Event")]
        public UnityCollisionEvent ColliderStayed;

        private void OnCollisionStay(Collision collision)
        {
            if (PhysicsEvent != null)
                PhysicsEvent.Raise(collision);

            ColliderStayed.Invoke(collision);
        }
    }
}

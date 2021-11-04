using UnityEngine;
using System.Collections.Generic;
using HeathenEngineering.UIX.Serialization;

namespace HeathenEngineering.UIX
{
    public class LigatureAplicationData
    {
        public List<LigatureReference> appliedLigatures;
        public string originalString;
        public string resultingString;
        public GameObject targetGameObject;
        public Component targetBehaviour;
        public string targetField;
    }
}
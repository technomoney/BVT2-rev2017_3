using System;
using UnityEngine.Events;

namespace HeathenEngineering.UIX
{
    [Serializable]
    public class KeyboardKeyStrokeEvent : UnityEvent<Keyboard, KeyboardKey>
    { }
}

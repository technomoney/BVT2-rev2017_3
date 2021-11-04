using HeathenEngineering.Scriptable;
using UnityEngine;

public class ExampleDemo_RandomizeColor : MonoBehaviour
{
    public void RandomizeColor(ColorVariable target)
    {
        target.SetValue(Random.ColorHSV());
    }
}

using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
///     Holds a reference to the test to which this belongs.  Only job is to instantiate and initialize the test options
///     associated with that test
/// </summary>
public class TestOptionContainer : SerializedMonoBehaviour
{
    [InfoBox("Reference to the test from the test manager")]
    public Test m_test;


    public void Awake()
    {
        if (m_test == null)
        {
            Debug.LogError("Test object is null for TestOptionContainer!");
            return;
        }

        var options = GetComponentsInChildren<TestOption>().ToList();

        if (options == null || options.Count <= 0)
        {
            Debug.LogError("No options found for test option container!");
            return;
        }

        options.ForEach(o => o.Initialize(m_test));
    }
}
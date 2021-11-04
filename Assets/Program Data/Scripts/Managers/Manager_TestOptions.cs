using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Manager_TestOptions : SerializedMonoBehaviour
{
    private static Dictionary<TestType, TestOptionContainer> m_optionContainers;

    private void Start()
    {
        //go through all of the children transform and grab the test containers
        m_optionContainers = new Dictionary<TestType, TestOptionContainer>();
        foreach (Transform t in transform)
        {
            var container = t.GetComponent<TestOptionContainer>();
            if (container == null) continue;
            m_optionContainers.Add(container.m_test.m_type, container);
        }
    }

    /// <summary>
    ///     Gets the given T component type in the children of the test container of the given test type.
    /// </summary>
    /// <param name="testType">Type of test this is getting the option from</param>
    /// <typeparam name="T">Test_Option.Type</typeparam>
    /// <returns>object of type T or null if it wasn't found</returns>
    public static T GetTestOption<T>(TestType testType)
    {
        //now we need to look through the conainter and get the option with the given type
        var option = m_optionContainers[testType].GetComponentInChildren<T>();

        if (option != null) return option;

        Debug.LogError("Got a null result getting option for test: " + testType);
        return default(T);
    }
}
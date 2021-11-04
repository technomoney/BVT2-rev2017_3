using Sirenix.OdinInspector;
using UnityEngine;

public class TestOption : SerializedMonoBehaviour
{
    protected Test m_test;

    public virtual void Initialize(Test test)
    {
        m_test = test;

        if (m_test == null)
        {
            Debug.LogError("Test object is null for TestOptionContainer!");
            return;
        }

        m_test = test;
        m_test.event_selected += TestSelected;
    }

    /// <summary>
    ///     When our referenced test is selected we want to set all the current test options to their
    ///     last values
    /// </summary>
    protected virtual void TestSelected()
    {
    }

    /// <summary>
    ///     Returns the index of the in the given array that equals the given value
    ///     Caution when doing this with floats.. some weird shit could happen
    /// </summary>
    /// <param name="values">Array of the values we're searching</param>
    /// <param name="val">The actual value we want, which is assigned to </param>
    /// <typeparam name="T">Type of the array</typeparam>
    /// <returns>Index of the given value, -1 if it wasn't found..</returns>
    public int GetIndexOfOption<T>(T[] values, T val)
    {
        for (var x = 0; x < values.Length; x++)
            if (val.Equals(values[x]))
                return x;

        Debug.LogError("Problem in GetIndexOfOption()..");
        return -1;
    }
}
using UnityEngine;

public class Manager_Inactivity : MonoBehaviour
{
    //how long, in seconds before we log off/quit
    private static readonly float m_maxInactivityTime = 3600;

    private float m_counter;

    // Use this for initialization
    private void Start()
    {
        m_counter = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            m_counter = 0;
            return;
        }

        m_counter += Time.deltaTime;

        if (m_counter < m_maxInactivityTime) return;

        //we've hit our max time, so quit/log off, do whatever we want to here..
        Application.Quit();
    }
}
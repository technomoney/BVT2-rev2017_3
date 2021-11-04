using Sirenix.OdinInspector;
using UnityEngine;

public class BP_HoverTarget : SerializedMonoBehaviour
{
    public static BP_HoverTarget Instance;
    private bool m_isMoving;
    public RectTransform m_rectTransform;
    private float m_speed, m_bearing;


    public void StartMoving(float speed, float bearing)
    {
        m_isMoving = true;
        m_speed = speed;
        m_bearing = bearing;
    }

    public void StopMoving()
    {
        m_isMoving = false;
    }

    private void Update()
    {
        if (!m_isMoving) return;

        //adjust our position
        var inc = new Vector2(Mathf.Cos(m_bearing) * m_speed, Mathf.Sin(m_bearing) * m_speed);
        m_rectTransform.anchoredPosition += inc;
    }

    public void ChangeSize(float newSize)
    {
        //this is likely to happen since the program usually starts with this object disabled..
        if (m_rectTransform == null)
        {
            m_rectTransform = GetComponent<RectTransform>();
            Instance = this;
        }

        m_rectTransform.sizeDelta = new Vector2(newSize, newSize);

        //change the size of the collider as well
        GetComponent<CircleCollider2D>().radius = newSize / 2;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.Equals("EdgeCollider")) return;

        //when we hit the edge collider we want to bounce back the way we came
        //todo this is a ping pong bounce effect, and may or may not be what we want in the end..
        m_bearing += 3.14f;
    }
}
using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LabelPositon
{
    Left,
    Right,
    Center
}

public class Graph_Bar_Entry : SerializedMonoBehaviour
{
    public float m_growSpeed;
    public Image m_image;
    private float m_maxSize, m_height;
    [HideInEditorMode] public RectTransform m_rectTransform;
    public TextMeshProUGUI m_text_label, m_text_title;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_maxSize = Mathf.Abs(m_rectTransform.sizeDelta.x);
        m_height = m_rectTransform.sizeDelta.y;

        m_rectTransform.sizeDelta = new Vector2(0, m_height);
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetValue(float value, float max, Color color, string title)
    {
        //determine our final physical size based on the given max and value
        var ratio = value / max;
        var finalSize = ratio * m_maxSize;

        m_image.color = color;
        m_text_title.text = title;

        StartCoroutine(StartMoving(finalSize, value));
    }

    private IEnumerator StartMoving(float finalSize, float value)
    {
        float i = 0;
        var initial = new Vector2(0, m_height);
        var final = new Vector2(finalSize, m_height);
        while (i < 1)
        {
            m_rectTransform.sizeDelta = Vector2.Lerp(initial, final, i);
            m_text_label.text = (value * i).ToString("0.0");
            i += Time.deltaTime * m_growSpeed;
            yield return null;
        }

        m_rectTransform.sizeDelta = new Vector2(finalSize, m_height);
        m_text_label.text = value.ToString();
    }
}
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DataPoint : SerializedMonoBehaviour
{
    [HideInEditorMode] public float m_data_x, m_data_y;
    public RectTransform m_rectTransform;
    public TextMeshProUGUI m_text_label_upper, m_text_label_lower;
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Summary : MonoBehaviour
{
    public static Summary Instance;

    public Button m_button_close;

    public TextMeshProUGUI m_text_info;

    private Vector2 m_vec2_hiddenPosition;

    // Use this for initialization
    private void Start()
    {
        Instance = this;
        m_vec2_hiddenPosition = GetComponent<RectTransform>().anchoredPosition;
        m_button_close.onClick.AddListener(Hide);
    }

    public void Show(string info)
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        m_text_info.text = info;
    }

    public void Hide()
    {
        GetComponent<RectTransform>().anchoredPosition = m_vec2_hiddenPosition;
    }
}
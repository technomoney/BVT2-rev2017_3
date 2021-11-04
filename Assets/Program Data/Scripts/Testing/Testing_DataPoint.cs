using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Testing_DataPoint : SerializedMonoBehaviour
{
    public RectTransform rectTransform;
    public TextMeshProUGUI text;

    public float x, y;

    // Use this for initialization
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Clicked);
        rectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Clicked()
    {
        Debug.Log(x + ", " + y);
    }
}
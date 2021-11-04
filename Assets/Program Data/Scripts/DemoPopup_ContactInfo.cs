using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class DemoPopup_ContactInfo : SerializedMonoBehaviour
{
    public void Initialize(TimeSpan daysRemaining)
    {
        GetComponent<Button>().onClick.AddListener(Clicked);

        transform.Find("Text_DaysRemaining").GetComponent<TextMeshProUGUI>().text =
            "This is a demo version: " + daysRemaining.Days + " days remaining.";

        gameObject.SetActive(false);
    }

    private void Clicked()
    {
        gameObject.SetActive(false);
    }
}
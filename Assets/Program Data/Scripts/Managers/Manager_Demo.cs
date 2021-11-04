using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Demo : SerializedMonoBehaviour
{
    private TimeSpan daysRamaining;
    public int m_demoLength;
    public bool m_deployAsDemo;
    public DemoPopup_ContactInfo m_infoPopup;

    private void Start()
    {
        if (!m_deployAsDemo)
        {
            //we can do this here mainly for the editor mode when testing..
            //we don't want this showing in case it is
            m_infoPopup.gameObject.SetActive(false);
            transform.Find("Demo Text").gameObject.SetActive(false);

            PlayerPrefs.DeleteKey("TrialEndDate");
            return;
        }

        //we're a demo, so see if this is the first time we're running the program
        var expiration = PlayerPrefs.GetString("TrialEndDate");
        daysRamaining = new TimeSpan();

        if (string.IsNullOrEmpty(expiration))
        {
            Debug.Log("In trial mode, no date entry found");
            //this means this the the first run so we have to make the entry..
            //or something wacky happened and we lost the entry..
            //so make the entry with today's date
            if (m_demoLength <= 0) m_demoLength = 30;
            expiration = DateTime.Today.AddDays(m_demoLength).ToString();
            Debug.Log("Adding demo expiration date: " + expiration);
            PlayerPrefs.SetString("TrialEndDate", expiration);
        }

        daysRamaining = DateTime.Parse(expiration) - DateTime.Today;
        if (daysRamaining.Days <= 0)
        {
            Debug.Log("Trial is expired");

            //hide the demo text, we' don't need it when the trial is expired
            transform.Find("Demo Text").gameObject.SetActive(false);
            //show the trial blocker
            transform.Find("Demo Blocker").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.Log("In trial mode: expiration ok");

            //set the text near the main menu to show how many days remain
            transform.Find("Demo Text").GetComponent<TextMeshProUGUI>().text =
                "Demo Version - " + daysRamaining.Days + " days remain";
            transform.Find("Demo Text").GetComponent<Button>().onClick.AddListener(ButtonPushed_InfoText);
        }

        //initialize the contact info
        m_infoPopup.Initialize(daysRamaining);
    }

    private void ButtonPushed_InfoText()
    {
        //when we push the info text show the info popup
        transform.Find("Contact Popup").gameObject.SetActive(true);
    }
}
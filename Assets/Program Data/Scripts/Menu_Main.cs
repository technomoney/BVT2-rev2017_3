using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Main : MonoBehaviour
{
    public static Menu_Main Instance;
    public Button m_button_subPanel;

    public Color m_color_selectedTextIcon, m_color_selectedButton, m_color_unselectedTextIcon, m_color_unselectedButton;

    //need references to each set of test options so we can show/hide
    public GameObject[] m_gameObj_testOptions;

    public int m_initiallySelectedTestIndex;

    public List<Button> m_list_testButtons;
    public MenuPanel m_menu_sub_options;

    // Use this for initialization
    public void Initialize()
    {
        Instance = this;
        m_button_subPanel.onClick.AddListener(ToggleSubPanel);

        m_list_testButtons[0].onClick.AddListener(delegate { ButtonPushed_Test(0); });
        m_list_testButtons[1].onClick.AddListener(delegate { ButtonPushed_Test(1); });
        m_list_testButtons[2].onClick.AddListener(delegate { ButtonPushed_Test(2); });
        m_list_testButtons[3].onClick.AddListener(delegate { ButtonPushed_Test(3); });
        m_list_testButtons[4].onClick.AddListener(delegate { ButtonPushed_Test(4); });
        m_list_testButtons[5].onClick.AddListener(delegate { ButtonPushed_Test(5); });
        m_list_testButtons[6].onClick.AddListener(delegate { ButtonPushed_Test(6); });
        m_list_testButtons[7].onClick.AddListener(delegate { ButtonPushed_Test(7); });
        m_list_testButtons[8].onClick.AddListener(delegate { ButtonPushed_Test(8); });
        m_list_testButtons[9].onClick.AddListener(delegate { ButtonPushed_Test(9); });

        //we want speed selected by default
        ButtonPushed_Test(m_initiallySelectedTestIndex, false);
    }

    private void ButtonPushed_Test(int testIndex, bool openPanel = true)
    {
        HandleButtonHighlighting(testIndex);

        //this handles the behavior of opening and closing the settings tray
        if (openPanel && m_menu_sub_options.m_isOpen)
        {
            //the panel is open let's see if we're clicking the currently active test, if so, close it
            if (testIndex == 0 && Manager_Test.Instance.m_selectedTestType == TestType.Speed)
                ToggleSubPanel();
            else if (testIndex == 1 && Manager_Test.Instance.m_selectedTestType == TestType.Peripheral)
                ToggleSubPanel();
            else if (testIndex == 2 && Manager_Test.Instance.m_selectedTestType == TestType.Sequence)
                ToggleSubPanel();
            else if (testIndex == 3 && Manager_Test.Instance.m_selectedTestType == TestType.Reaction)
                ToggleSubPanel();
            else if (testIndex == 4 && Manager_Test.Instance.m_selectedTestType == TestType.Balance)
                ToggleSubPanel();
            else if (testIndex == 5 && Manager_Test.Instance.m_selectedTestType == TestType.GoNoGo)
                ToggleSubPanel();
            else if (testIndex == 6 && Manager_Test.Instance.m_selectedTestType == TestType.Flash)
                ToggleSubPanel();
            else if (testIndex == 7 && Manager_Test.Instance.m_selectedTestType == TestType.Rhythm)
                ToggleSubPanel();
            else if (testIndex == 8 && Manager_Test.Instance.m_selectedTestType == TestType.Contrast)
                ToggleSubPanel();
            else if (testIndex == 9 && Manager_Test.Instance.m_selectedTestType == TestType.Multi)
                ToggleSubPanel();
        }
        //commenting this out stop the drawer from opening when a test button is pushed, this way the drawer can
        //only be opened by pushing the tab
        //else if (!m_menu_sub_options.m_isOpen && openPanel)
        //  ToggleSubPanel();

        //initially just hide all options panels
        foreach (var g in m_gameObj_testOptions)
            g.SetActive(false);

        //we generally want to hide the balance plate cursor
        BalancePlateCursor.Instance.SetCursorVisibility(false);

        //then enable the correct one
        m_gameObj_testOptions[testIndex].SetActive(true);

        //set the active test in the manager
        Manager_Test.Instance.SetActiveTest(testIndex);

        //then make sure we call the selected function on this test
        Manager_Test.Instance.m_selectedTest.Selected();
    }

    private void HandleButtonHighlighting(int buttonIndex)
    {
        //set everything to unselected
        foreach (var b in m_list_testButtons)
        {
            b.transform.Find("Image_BG2").GetComponent<Image>().color = m_color_unselectedButton;
            b.transform.Find("Image_Icon").GetComponent<Image>().color = m_color_unselectedTextIcon;
            b.transform.Find("Text_Label").GetComponent<TextMeshProUGUI>().color = m_color_unselectedTextIcon;
        }

        //then set the selected colors
        m_list_testButtons[buttonIndex].transform.Find("Image_BG2").GetComponent<Image>().color =
            m_color_selectedButton;
        m_list_testButtons[buttonIndex].transform.Find("Image_Icon").GetComponent<Image>().color =
            m_color_selectedTextIcon;
        m_list_testButtons[buttonIndex].transform.Find("Text_Label").GetComponent<TextMeshProUGUI>().color =
            m_color_selectedTextIcon;
    }

    private void ToggleSubPanel()
    {
        //comments here stop the arrow animation..
        if (m_menu_sub_options.m_isOpen)
        {
            m_button_subPanel.transform.GetChild(0).GetComponent<RectTransform>().localRotation =
                Quaternion.Euler(0, 0, 90);
            m_menu_sub_options.Close();
        }
        else
        {
            m_button_subPanel.transform.GetChild(0).GetComponent<RectTransform>().localRotation =
                Quaternion.Euler(0, 0, -90);
            m_menu_sub_options.Open();
        }
    }
}
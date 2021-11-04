using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Manager_Fullscreen : SerializedMonoBehaviour
{
    public static Manager_Fullscreen Instance;
    private Button m_button;

    public Button m_button_maxmize;

    // Use this for initialization
    private void Start()
    {
        Instance = this;
        //this is annoying to have enabled when using the editor, so this will make sure we can hook the event
        //with it disabled in editor mode
        transform.GetChild(0).gameObject.SetActive(true);
        m_button = GetComponentInChildren<Button>();
        m_button.onClick.AddListener(ButtonPushed_Giant);
        m_button_maxmize.onClick.AddListener(ButtonPushed_Maximize);
        //hide the button
        m_button.gameObject.SetActive(false);
    }

    public void EnableFullscreenMode()
    {
        //hide absolutely everything
        Menu_Main.Instance.gameObject.SetActive(false);
        Manager_Test.Instance.SetMenuVisibility(false);
        Menu_Reports.Instance.gameObject.SetActive(false);
        Banner_StartSummary.Instance.gameObject.SetActive(false);
        Manager_SummaryGraphs.Instance.HideAll();
        PatientDisplay.Instance.Show(false);
        Manager_Targets.Instance.gameObject.SetActive(false);

        //show the button
        m_button.gameObject.SetActive(true);
    }

    public void DisableFullscreenMode()
    {
        //show everything
        Menu_Main.Instance.gameObject.SetActive(true);
        Manager_Test.Instance.SetMenuVisibility(true);
        Menu_Reports.Instance.gameObject.SetActive(true);
        Banner_StartSummary.Instance.gameObject.SetActive(true);
        Manager_SummaryGraphs.Instance.HideAll();
        PatientDisplay.Instance.Show(true);
        Manager_Targets.Instance.gameObject.SetActive(true);

        //hide the button
        m_button.gameObject.SetActive(false);
    }

    private void ButtonPushed_Giant()
    {
        DisableFullscreenMode();
    }

    private void ButtonPushed_Maximize()
    {
        EnableFullscreenMode();
    }
}
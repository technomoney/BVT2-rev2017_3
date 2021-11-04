using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemWindow : BVT_Window
{
    public static SystemWindow Instance;
    public Button m_button_close, m_button_exit, m_button_show, m_button_support, m_button_bp, m_button_manageUsers;
    public TextMeshProUGUI m_text_currentUser;
    public TextMeshProUGUI m_text_version;
    public Toggle m_toggle_recordData;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        UpdateCurrentVersionText();
        m_button_show.onClick.AddListener(Show);
        m_button_close.onClick.AddListener(Hide);
        m_button_exit.onClick.AddListener(ButtonPushed_Exit);
        m_button_support.onClick.AddListener(ShowSupportWindow);
        m_button_bp.onClick.AddListener(ShowBpWindow);
        m_toggle_recordData.onValueChanged.AddListener(delegate { ToggleRecordData(m_toggle_recordData); });
        m_button_manageUsers.onClick.AddListener(ShowUserWindow);
    }

    public override void Show()
    {
        base.Show();
        UpdateCurrentUserText();
    }

    private void ButtonPushed_Exit()
    {
        Window_GenericConfirmation.Inst.Show("Confirm Exit",
            "Are you sure you want to exit the BVT software?",
            Confirm_Exit, null);
    }

    private void Confirm_Exit()
    {
        Application.Quit();
    }

    private void ShowSupportWindow()
    {
        var status = LicenseKeyManager.Instance.GetKeyStatus();
        var key = LicenseKeyManager.Instance.m_validatedKey;
        if (key.Length == 20)
            //if we're a correct length key we can put the dashes back in so it looks nice
            //we can do this check in case we're bypassing the key check for testing or something to stop it
            //from breaking
            key = key.Substring(0, 4) + "-" +
                  key.Substring(4, 4) + "-" +
                  key.Substring(8, 4) + "-" +
                  key.Substring(12, 4) + "-" +
                  key.Substring(16, 4);

        var str = "\nLicense Key: " + key + "\nLicense Status: " + (status ? "Ok" : "Invalid") + "\n" +
                  "\nContact Support:\nsupport@bertec.com   |   (614)450-0331";
        Window_GenericMessage.Inst.Show("Support", str);
    }

    private void ShowBpWindow()
    {
        Window_BalancePlate.Instance.Show();
    }

    private void ToggleRecordData(Toggle t)
    {
        Manager_Test.Instance.m_recordBpData = t.isOn;
    }

    private void ShowUserWindow()
    {
        Manager_Users.Instance.ToggleVisibility();
    }

    public void UpdateCurrentUserText()
    {
        m_text_currentUser.text = Manager_Users.Instance.m_currentUser == null
            ? "None"
            : Manager_Users.Instance.m_currentUser.m_accountName;
    }

   public void UpdateCurrentVersionText()
   {
      try
      {
         VersionNumberResource vn = VersionNumberResource.Load();
         m_text_version.text = "v" + vn.FileVersion;
      }
      catch (System.Exception ex)
      {
         Debug.Log("Unable to set version #: " + ex);
      }
   }
}
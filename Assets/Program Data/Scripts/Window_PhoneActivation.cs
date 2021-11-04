using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_PhoneActivation : MonoBehaviour
{
    public static Window_PhoneActivation Instance;
    public Button m_button_activate, m_button_back;
    private Vector2 m_hidePos;
    private bool m_isShowing;
    public Vector2 m_showPos;
    public TextMeshProUGUI m_text_systemId, m_text_key;
    public TMP_InputField m_input_key;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_hidePos = GetComponent<RectTransform>().anchoredPosition;
        m_isShowing = false;

        m_button_activate.onClick.AddListener(ButtonPushed_Activate);
        m_button_back.onClick.AddListener(ButtonPushed_Back);
    }

    public void Show(string id = "", string key = "")
    {
        m_isShowing = !m_isShowing;

        GetComponent<RectTransform>().anchoredPosition = m_isShowing ? m_showPos : m_hidePos;

        if (!m_isShowing) return;

        if (id.IsNullOrWhitespace())
        {
            Debug.LogError("Phone Window showing with empty id");
            return;
        }

        m_text_systemId.text = "Your System ID: " + id;
        m_text_key.text = "Your Product Key: " + key.ToUpper();
    }

    public void ButtonPushed_Back()
    {
        Show();
    }

    public void ButtonPushed_Activate()
    {
        //see if this is a legit key/activation pair
        if (LicenseKeyManager.Instance.CheckPhoneActivation(m_input_key.text))
            Show();
        else
        {
            Window_GenericMessage.Inst.Show("Activation Key Error",
                "Unable to validate key, please try again.");
        }
    }
}
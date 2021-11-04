using HeathenEngineering.UIX;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_UserLogin : BVT_Window
{
    public static Window_UserLogin Instance;
    [BoxGroup("On Screen Keyboard")] public Button m_button_keyboardToggle, m_button_keyboard_hide;
    public Button m_button_login, m_button_cancel;
    public TMP_InputField m_field_pass;
    private bool m_forEdit;
    private Vector2 m_keyboard_hidePosition;
    private bool m_keyboard_isSowing;
    [BoxGroup("On Screen Keyboard")] public Vector2 m_keyboard_showPosition;
    [BoxGroup("On Screen Keyboard")] public KeyboardOutputManager m_onScreenKeyboard;
    public TextMeshProUGUI m_text_title, m_text_warning;
    private UserAccount m_user;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        Instance = this;
        m_button_cancel.onClick.AddListener(Hide);
        m_button_login.onClick.AddListener(ButtonPushed_Login);
        m_button_keyboardToggle.onClick.AddListener(KeyboardToggle);
        m_button_keyboard_hide.onClick.AddListener(KeyboardHide);

        m_keyboard_hidePosition = m_onScreenKeyboard.GetComponent<RectTransform>().anchoredPosition;
        m_keyboard_isSowing = false;

        m_field_pass.onSelect.AddListener(delegate { SetKeyboardFocus(m_field_pass); });
    }

    public override void Show()
    {
        base.Show();

        m_field_pass.text = string.Empty;
        m_text_warning.text = string.Empty;
    }

    public void SetUser(UserAccount user, bool forEdit = false)
    {
        m_forEdit = forEdit;
        m_user = user;
        m_text_title.text = "Enter Password for Operator: " + m_user.m_accountName;
    }

    private void ButtonPushed_Login()
    {
        //when we push login we need to check if the entered text matches the pass for this user
        var enteredPass = m_field_pass.text;
        if (!enteredPass.Equals(m_user.m_password))
        {
            //show our warning text
            m_text_warning.text = "Incorrect Password";
            return;
        }

        //otherwise, log in as this user
        if (m_forEdit)
            Manager_Users.Instance.OpenUserForEditing(m_user, m_user.m_isAdmin);
        Manager_Users.Instance.SetCurrentUser(m_user);

        //then hide this
        ToggleVisibility();
        //and hide the keyboard in case we're showing it
        KeyboardHide();
    }

    private void SetKeyboardFocus(TMP_InputField field)
    {
        m_onScreenKeyboard.linkedGameObject = field.gameObject;
        m_onScreenKeyboard.linkedBehaviour = field.GetComponent<TMP_InputField>();
    }

    private void KeyboardToggle()
    {
        m_keyboard_isSowing = !m_keyboard_isSowing;
        if (m_keyboard_isSowing)
            m_onScreenKeyboard.GetComponent<RectTransform>().anchoredPosition = m_keyboard_showPosition;
        else KeyboardHide();
    }

    private void KeyboardHide()
    {
        m_keyboard_isSowing = false;
        m_onScreenKeyboard.GetComponent<RectTransform>().anchoredPosition = m_keyboard_hidePosition;
    }
}
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_AddUser : MonoBehaviour
{
    public Button m_button_cancel, m_button_add;

    /// <summary>
    ///     this is a special flag we need for when we're editing the amdin account.  This needs to be set so when the entry is
    ///     serialized to xml, it is set as admin instead of a nomral user
    /// </summary>
    private bool m_editingAdmin;

    public TMP_InputField m_field_name_first,
        m_field_name_last,
        m_field_title,
        m_field_accountName,
        m_field_pass,
        m_field_confirm;

    private Vector2 m_hiddenPos;
    private bool m_isShowing;
    public TextMeshProUGUI m_text_title;
    public TextMeshProUGUI m_text_warning;

    // Use this for initialization
    private void Start()
    {
        m_hiddenPos = GetComponent<RectTransform>().anchoredPosition;
        m_button_cancel.onClick.AddListener(ToggleVisibility);
        m_button_add.onClick.AddListener(AddUser);
    }

    public void ToggleVisibility()
    {
        m_isShowing = !m_isShowing;
        ClearFields();
        GetComponent<RectTransform>().anchoredPosition = m_isShowing ? Vector2.zero : m_hiddenPos;
        m_text_title.text = "Add New Operator";

        if (m_isShowing) GetComponent<TabThroughFields>().ActivateFirstField();
        else GetComponent<TabThroughFields>().Disable();
    }

    public void ShowWithFilledFields(UserAccount user, bool editingAdmin = false)
    {
        m_text_title.text = "Edit Operator";

        m_editingAdmin = editingAdmin;
        m_isShowing = true;
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ClearFields();

        m_field_name_first.text = user.m_name_first;
        m_field_name_last.text = user.m_name_last;
        m_field_title.text = user.m_title;
        m_field_accountName.text = user.m_accountName;
        m_field_accountName.interactable = false;
        m_field_pass.text = user.m_password;
        m_field_confirm.text = string.Empty;
    }

    private void ClearFields()
    {
        m_field_name_first.text = string.Empty;
        m_field_name_last.text = string.Empty;
        m_field_title.text = string.Empty;
        m_field_accountName.text = string.Empty;
        m_field_accountName.interactable = true;
        m_field_pass.text = string.Empty;
        m_field_confirm.text = string.Empty;
        m_text_warning.text = string.Empty;
    }

    private void AddUser()
    {
        //make sure our junk is filled out
        //for now, the only things we'll require is user/pass
        if (m_field_accountName.text.Equals(string.Empty)) return;
        //show the warning text

        //if there is a password, make sure they match
        if (!m_field_pass.text.Equals(string.Empty))
            if (!m_field_pass.text.Equals(m_field_confirm.text))
            {
                //show the warning text
                m_text_warning.text = "Passwords do not match";
                return;
            }

        var first = m_field_name_first.text;
        var last = m_field_name_last.text;
        var title = m_field_title.text;
        var user = m_field_accountName.text;
        var pass = m_field_pass.text;

        //todo should not allow an account with name or title be Admin 

        var newUser = new UserAccount(first, last, title, user, pass, m_editingAdmin);
        //make a new file to store this serialized user account
        SerializeNewUser(newUser);

        //redraw the display to show our newly added user
        Manager_Users.Instance.PopulateUserList();
        //this will force the upper fields to update to reflect any changes
        Manager_Users.Instance.SetCurrentUser(Manager_Users.Instance.m_currentUser);
        ToggleVisibility();
    }

    public void SerializeNewUser(UserAccount user)
    {
        var writer = new XmlSerializer(typeof(UserAccount));
        var file = File.Create(Manager_Users.USER_DIRECTORY + "/user_" + user.m_accountName);
        writer.Serialize(file, user);
        file.Close();
    }

    public void MakeAdmin()
    {
        var admin = new UserAccount("-", "-", "Administrator", "Admin", "password", true);
        //save this as a new file in our user directory
        var writer = new XmlSerializer(typeof(UserAccount));
        var file = File.Create(Manager_Users.USER_DIRECTORY + "/user_Admin");
        writer.Serialize(file, admin);
        file.Close();
    }
}
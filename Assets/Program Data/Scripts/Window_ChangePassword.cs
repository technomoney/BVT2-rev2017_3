using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine.UI;

public class Window_ChangePassword : BVT_Window
{
    public Button m_button_ok, m_button_cancel;
    public TMP_InputField m_field_old, m_field_new1, m_field_new2;
    public TextMeshProUGUI m_text_warning, m_text_title;
    private UserAccount m_user;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        m_button_cancel.onClick.AddListener(ToggleVisibility);
        m_button_ok.onClick.AddListener(ButtonPushed_Ok);
    }

    public override void ToggleVisibility()
    {
        base.ToggleVisibility();

        m_field_old.text = string.Empty;
        m_field_new1.text = string.Empty;
        m_field_new2.text = string.Empty;
        m_text_warning.text = string.Empty;
    }

    public void SetUser(UserAccount user)
    {
        m_user = user;
        m_text_title.text = "Change Password for Operator: " + m_user.m_accountName;
    }

    private void ButtonPushed_Ok()
    {
        var old = m_field_old.text;
        var new1 = m_field_new1.text;
        var new2 = m_field_new2.text;

        //make sure new password 1 and 2 match
        if (!new1.Equals(new2))
        {
            m_text_warning.text = "New Passwords do not match";
            return;
        }

        //make sure the old password matches
        if (!old.Equals(m_user.m_password))
        {
            m_text_warning.text = "Old Password is Incorrect";
            return;
        }

        //now we want to update this user, so find them in the user folder
        var userPath = Manager_Users.Instance.GetUserFilePath(m_user);
        if (userPath == null) return;

        //deserialize this file to a temp user object
        var reader = new XmlSerializer(typeof(UserAccount));
        var file = new StreamReader(userPath);
        var user = (UserAccount) reader.Deserialize(file);
        file.Close();

        //change the password
        user.m_password = new1;

        //then reserialize and save this update file
        var writer = new XmlSerializer(typeof(UserAccount));
        var fileStream = File.Create(Manager_Users.USER_DIRECTORY + "/user_" + user.m_accountName);
        writer.Serialize(fileStream, user);
        fileStream.Close();

        //then force the manager to repopulate and close this window
        Manager_Users.Instance.PopulateUserList();
        ToggleVisibility();
    }
}
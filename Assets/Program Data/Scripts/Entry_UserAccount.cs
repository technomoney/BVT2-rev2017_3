using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Entry_UserAccount : MonoBehaviour
{
    private UserAccount m_account;
    public Button m_button_trash, m_button_edit;
    public TextMeshProUGUI m_text_accountName, m_text_name, m_text_title;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Pushed);
        m_button_trash.onClick.AddListener(Trash);
        m_button_edit.onClick.AddListener(Edit);
    }

    public void Initialize(UserAccount account)
    {
        m_account = account;
        m_text_name.text = m_account.m_name_first + " " + m_account.m_name_last;
        m_text_title.text = m_account.m_title;
        m_text_accountName.text = m_account.m_accountName;
    }

    private void Pushed()
    {
        //show the log in window for this user
        Window_UserLogin.Instance.SetUser(m_account);
        Window_UserLogin.Instance.ToggleVisibility();
    }

    private void Trash()
    {
        //before we trash this, show the confirmation window
        //Window_ConfirmUserDelete.Instance.Show(m_account);
        Window_GenericConfirmation.Inst.Show("Confirm Delete Operator",
            "Are you sure you want to remove this operator?  This cannot be undone.", ConfirmDelete, null);
    }

    private void ConfirmDelete()
    {
        Manager_Users.Instance.ConfirmDeleteUser(m_account);
    }

    private void Edit()
    {
        Manager_Users.Instance.ButtonPushed_EditUser(m_account);
    }

    public void SetTrashButtonVisibility(UserAccount currentUser)
    {
        if (m_account.m_isAdmin)
        {
            //the trash button should never be visible on the admin entry
            m_button_trash.gameObject.SetActive(false);
            return;
        }

        //we only show the trash can on a normal account if the current user is not null (obvs) and the admin
        //we do the null check here to avoid an awkward check in Manager_Users
        var show = currentUser != null && currentUser.m_isAdmin;

        m_button_trash.gameObject.SetActive(show);
    }
}
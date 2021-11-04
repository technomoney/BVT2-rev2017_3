using System;

[Serializable]
public class UserAccount
{
    public string m_accountName, m_name_first, m_name_last, m_title, m_password;
    public bool m_isAdmin;

    public UserAccount()
    {
        //need a default constructor for xml serialization even if we don't use it
    }

    public UserAccount(string nameFirst, string nameLast, string title, string accountName, string pass,
        bool admin = false)
    {
        m_name_first = nameFirst;
        m_name_last = nameLast;
        m_title = title;
        m_accountName = accountName;
        m_password = pass;
        m_isAdmin = admin;
    }

    public void SetAsAdmin()
    {
        m_isAdmin = true;
    }
}
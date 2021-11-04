using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Users : MonoBehaviour
{
    public static string USER_DIRECTORY;
    public static Manager_Users Instance;
    private List<Entry_UserAccount> m_accountEntries;
    public Button m_button_addUser, m_button_close, m_button_changePass, m_button_exit;
    public UserAccount m_currentUser;
    private Vector2 m_hiddenPos;
    private bool m_isShowing;
    public TextMeshProUGUI m_text_currentAccountName, m_text_currentUserName, m_text_currentUserTitle;
    public Transform m_trans_entryContent;
    private List<string> m_userFilePaths;
    public Window_AddUser m_window_addUser;
    public Window_ChangePassword m_window_changePass;
    public Entry_UserAccount pfb_entry_userAccount;

    /// <summary>
    /// keeps track if this is the first time this session we're logging in so we can auto close the op window
    /// </summary>
    private bool m_firstLogin = true;

    // Use this for initialization
    private void Start()
    {
        Debug.Log("User manager starting");
        Instance = this;
        m_hiddenPos = GetComponent<RectTransform>().anchoredPosition;
        m_button_addUser.onClick.AddListener(ButtonPushed_AddUser);
        m_button_close.onClick.AddListener(ToggleVisibility);
        m_button_changePass.onClick.AddListener(ButtonPushed_ChangePass);
        m_button_exit.onClick.AddListener(ButtonPushed_Exit);

        m_currentUser = null;
        //first, make sure our user directory exists
        var m_path_userDir = Application.persistentDataPath + "/UserData";

        //if not, make it
        if (!Directory.Exists(m_path_userDir))
            Directory.CreateDirectory(m_path_userDir);

        USER_DIRECTORY = m_path_userDir;
        
        //CleanUpTempFiles();

        //get all of our user files
        GetUserAccountFiles();
        Debug.Log("Got " + m_userFilePaths.Count + " user account files");

        //remake the admin account if there isn't one there
        if (!AdminAccountPresent())
            m_window_addUser.MakeAdmin();
    }

    /// <summary>
    /// this should only be needed while testing we can scrub the users folder for any
    /// left over temp files and nuke them
    /// </summary>
    private void CleanUpTempFiles()
    {
        var dirInfo = new DirectoryInfo(USER_DIRECTORY);
        var files = dirInfo.GetFiles();
        var markedForDeletion = new List<FileInfo>();
        foreach (var f in files)
            if(f.Name.Substring(f.Name.Length-4, 4).Equals("temp"))
                markedForDeletion.Add(f);
        

        for (int x = markedForDeletion.Count - 1; x >= 0; x--)
            File.Delete(markedForDeletion[x].FullName);
    }

    private bool AdminAccountPresent()
    {
        foreach (var path in m_userFilePaths)
        {
            var file = new StreamReader(path);
            var content = file.ReadToEnd();
            file.Close();
            //we're going to assume this is already encrypted data....
            var decryStr = EncryptDecrypt.ProcessString(content);
            //write this to a temp file
            var writer = new StreamWriter(path + "_temp");
            writer.Write(decryStr);
            writer.Close();
            //deserialize it to read it
            file = new StreamReader(path + "_temp");
            var xmlReader = new XmlSerializer(typeof(UserAccount));
            var user = (UserAccount) xmlReader.Deserialize(file);
            file.Close();

            var isAdmin = user.m_isAdmin;
            
            //nuke the temp file we just made
            File.Delete(path + "_temp");
            
            if (user.m_isAdmin) return true;
        }

        return false;
    }

    public void ButtonPushed_ChangePass()
    {
        m_window_changePass.SetUser(m_currentUser);
        m_window_changePass.Show();
    }

    public void SetCurrentUser(UserAccount user)
    {
        m_currentUser = user;
        m_text_currentAccountName.text = "Current User: " + m_currentUser.m_accountName;
        m_text_currentUserName.text = "Name: " + (m_currentUser.m_isAdmin
                                          ? "-"
                                          : m_currentUser.m_name_first + " " + m_currentUser.m_name_last);
        m_text_currentUserTitle.text = "Title: " + m_currentUser.m_title;
        SystemWindow.Instance.UpdateCurrentUserText();

        //show the close button whenever a user is set, this may be hidden if this is the first user to be logging
        //in when the software is started
        m_button_close.gameObject.SetActive(true);

        m_accountEntries.ForEach(e => e.SetTrashButtonVisibility(m_currentUser));

        if (m_firstLogin)
        {
            ToggleVisibility();
            m_firstLogin = false;
        }
    }

    public void ToggleVisibility()
    {
        m_isShowing = !m_isShowing;

        GetComponent<RectTransform>().anchoredPosition = m_isShowing ? Vector2.zero : m_hiddenPos;

        if (!m_isShowing) return;
        //when we show the window, populate the user list
        PopulateUserList();
        //show the close button in case it is still hidden
        m_button_close.gameObject.SetActive(true);
    }

    public void ShowOnStartUp()
    {
        m_isShowing = true;
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        PopulateUserList();
        //on startup we need to hide the close button so someone has to be logged in
        m_button_close.gameObject.SetActive(false);
    }

    private void GetUserAccountFiles()
    {
        var dirInfo = new DirectoryInfo(USER_DIRECTORY);
        var files = dirInfo.GetFiles();

        //get all files with the prefix "user_"
        m_userFilePaths = new List<string>();

        foreach (var f in files)
            if (f.Name.Contains("user_"))
                m_userFilePaths.Add(f.ToString());
        
        //when using encryption, we'll want to read in the files, decrypt and save them in a structure, and then
        //use that structure, not the actual file location any longer
    }

    private void ButtonPushed_AddUser()
    {
        //can be null if no one is logged in at startup
        if (m_currentUser == null) return;
        //when the add user button is pushed, we should only show the window if we're logged in as the admin
        if (!m_currentUser.m_isAdmin)
        {
            Window_GenericMessage.Inst.Show("Admin Warning", "Only the Administrator account can add new Operators");
            return;
        }

        m_window_addUser.ToggleVisibility();
    }

    public void ButtonPushed_EditUser(UserAccount user)
    {
        //we need to prompt for a password unless we're the admin
        if (m_currentUser != null && m_currentUser.m_isAdmin)
        {
            //just show the panel, no need for password entry
            //if we're actually editing the amdmin account, set the flag as such
            OpenUserForEditing(user, user.m_isAdmin);
        }
        else
        {
            //prompt for a password before showing it
            //show the log in window for this user
            Window_UserLogin.Instance.SetUser(user, true);
            Window_UserLogin.Instance.ToggleVisibility();
        }
    }

    public void OpenUserForEditing(UserAccount user, bool editingAdmin)
    {
        m_window_addUser.ShowWithFilledFields(user, editingAdmin);
    }

    public void PopulateUserList()
    {
        GetUserAccountFiles();

        //now go through our file list and make an entry for each one
        //nuke any existing ones
        if (m_accountEntries != null)
            m_accountEntries.ForEach(e => Destroy(e.gameObject));

        m_accountEntries = new List<Entry_UserAccount>();
        //we're going to assume the users are encrypted here so let's decrypt before deserializing them
        foreach (var path in m_userFilePaths)
        {
            var reader = new StreamReader(path);
            var xmlContent = reader.ReadToEnd();
           
            reader.Close();
            //todo possible cameron problem here
            //we can tell if it is plain xml by just looking at the very first character
            if (xmlContent.Substring(0, 5).Equals("<?xml"))
            {
                //this is a normal, non-encrypted xml file, this usually shouldn't happen but can if we're running
                //this on users that were created before encryption was switched on, or if a new user was just created
                //to make everything match up, we should serialize it here, and then we'll create the temp
                //normally below so everything syncs up nicely
                xmlContent = EncryptDecrypt.ProcessString(xmlContent);
                //now we'll write this back to the file we just opened, so now it will be an encrypted version of itself
                var w = new StreamWriter(path);
                w.Write(xmlContent);
                w.Close();
            }

            //todo there is almost certainly a better way to do this, but this will do for now
            //we need to decrypt this string, and save it back to the user file
            var encryptedStr = EncryptDecrypt.ProcessString(xmlContent);
            //then save it back to the file for normal reading below
            var writer = new StreamWriter(path + "_temp");
            writer.Write(encryptedStr);
            writer.Close();
        }

        foreach (var path in m_userFilePaths)
        {
            //deserialize the file here to a new user object
            var reader = new XmlSerializer(typeof(UserAccount));
            var file = new StreamReader(path + "_temp");
         
            var user = (UserAccount) reader.Deserialize(file);
            file.Close();

            //make the entry
            var entry = Instantiate(pfb_entry_userAccount, m_trans_entryContent);
            entry.Initialize(user);
            m_accountEntries.Add(entry);
            entry.SetTrashButtonVisibility(m_currentUser);
        }

        //now that we're read and store all of the current users, we can nuke our temp file
       foreach (var path in m_userFilePaths)
        {
            /*
            var file = new StreamReader(path);
            var normalString = file.ReadToEnd();
            file.Close();
            var encStr = EncryptDecrypt.ProcessString(normalString);
            //then rewrite it to the file
            var writer = new StreamWriter(path);
            writer.Write(encStr);
            writer.Close();
            */
            File.Delete(path + "_temp");
        }

    }

    public string GetUserFilePath(UserAccount user)
    {
        var dirInfo = new DirectoryInfo(USER_DIRECTORY);
        var files = dirInfo.GetFiles();

        foreach (var f in files)
            if (f.Name.Equals("user_" + user.m_accountName))
                return f.ToString();

        Debug.LogError("Could not find user path for user: " + user.m_accountName + ", returning null!");
        return null;
    }

    public void ConfirmDeleteUser(UserAccount user)
    {
        //find the user file and delete it
        var path = GetUserFilePath(user);
        if (!path.IsNullOrWhitespace())
            File.Delete(path);

        PopulateUserList();
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
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum KeyCheckState
{
    InitialCheck,
    ServerCommFailed,
    ServerSuccess,
    ServerRegFailed,
    WaitingOnFormFill,
    WaitingOnServerCheck,
    ExistingKeyValidated,
    Waiting
}

public class LicenseKeyManager : BVT_Window
{
    public static LicenseKeyManager Instance;
    [InfoBox("Enable this to remove our test key on load")]
    public bool TESTING_REMOVEKEY;
    public Button m_button_close, m_button_activateInternet, m_button_activatePhone;
    public Button m_button_override, m_button_fakeKey;
    public TMP_InputField[] m_inputFields;
    

    /// <summary>
    ///     We have to use an enum here to deal with the server and key check based events to avoid thread access violations
    ///     This should be able to be handled with an execute on main thread method, but unity sucks and like to hard crash
    ///     whenever an access violation error occurs and it makes it a bitch to debug and test, so this works for now
    ///     but should be a candidate for updating if we ever migrate to a newer version of unity
    /// </summary>
    private KeyCheckState m_keyCheckState;

    private BVTLicenseManager m_keyManager;
    public TMP_InputField[] m_reg_inputFields;
    public bool m_testingMode;
    public TextMeshProUGUI m_text_status, m_text_error, m_text_info;
    public string m_validatedKey;
    private static bool m_closedOut;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        m_closedOut = false;
        for (var x = 0; x < 5; x++)
        {
            var index = x;
            m_inputFields[x].onValueChanged.AddListener(delegate { TextFieldValueChanged(m_inputFields[index]); });
        }

        m_button_close.onClick.AddListener(ButtonPushed_Back);

        m_button_override.onClick.AddListener(ButtonPushed_Back);
        m_button_fakeKey.onClick.AddListener(ButtonPushed_FakeKey);

        m_button_activateInternet.onClick.AddListener(ButtonPushed_ActivateInternet);
        m_button_activatePhone.onClick.AddListener(ButtonPushed_ActivatePhone);

        //Hook our server events
        m_keyManager = new BVTLicenseManager();

        m_keyManager.OnServerCommunicationFailure += ServerCommunicationFailure;
        m_keyManager.OnServerKeyRegistrationFailed += ServerKeyRegistrationFailed;
        m_keyManager.OnServerKeyRegistrationSucceeded += ServerKeyRegistrationSucceeded;
        m_keyManager.OnServerFinished += ServerFinished;
        m_keyManager.OnLicenseKeyNowDeactivated += LicenseKeyNowDeactivated;

        m_keyManager.OnLicenseKeyCheckStarted += LicenseKeyCheckStarted;
        m_keyManager.OnLicenseKeyCheckWorking += LicenseKeyCheckWorking;
        m_keyManager.OnLicenseKeyCheckComplete += LicenseKeyCheckComplete;

        //start the key manager
        m_keyCheckState = KeyCheckState.InitialCheck;
        m_keyManager.StartManager();
        if (TESTING_REMOVEKEY)
            //m_keyManager.UpdateKeyringRemoveLicenseKey("SLVR-3ZY4-FJ6G-6WOF-2UAI");
            //m_keyManager.UpdateKeyringRemoveLicenseKey("AAAA-AEE7-45TF-XJYF-5FYB");
            //m_keyManager.UpdateKeyringRemoveLicenseKey("SLVR-TT2M-F46G-6WOF-2PYJ");
            m_keyManager.UpdateKeyringRemoveLicenseKey("SLVR-HAS4-FELL-XJYF-5TIP");

        m_keyManager.StartBackgroundChecking(false);

        var status = m_keyManager.CheckLicenseGroups(new[] {"BVTSilver"});
        
       

        //check for a valid key, otherwise pop up the window
        Debug.Log("Key Status: " + status);

        if (m_testingMode)
        {
            Debug.LogWarning("In License testing mode, bypassing any key checks...");
            return;
        }

        //if we do not have a valid key we need to hide the close button
        if (status != BVTLicenseManager.LicenseKeyStatus.OK)
        {
            m_button_close.gameObject.SetActive(false);
            //then pop up this window
            Show();
        }
        else
        {
            //if we are valid, hide the junk we don't need and set the status message
            m_button_close.gameObject.SetActive(false);
            m_text_info.gameObject.SetActive(false);
            m_button_activateInternet.gameObject.SetActive(false);
            m_button_activatePhone.gameObject.SetActive(false);
            m_inputFields.ForEach(f => f.gameObject.SetActive(false));
            m_text_status.text = "Product License Active";

            // The status says that the keys are ok, so we're good to go.
            m_keyCheckState = KeyCheckState.ExistingKeyValidated;
            Debug.Log(m_keyManager.KeyringFilename);

        }

        //testing
        Debug.Log(m_keyManager.SystemInstallationID);

        GetComponent<TabThroughFields>().ActivateFirstField();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.A) &&
            Input.GetKeyDown(KeyCode.D))
            ButtonPushed_FakeKey();

        switch (m_keyCheckState)
        {
            case KeyCheckState.Waiting:
                //generic pass through state...
                break;
            case KeyCheckState.InitialCheck:
                break;
            case KeyCheckState.ServerCommFailed:
                Window_GenericMessage.Inst.Show(
                    "Server Communication Error",
                    "Unable to reach license server, please check your internet connection and try again or activate using Phone validation.");
                m_keyCheckState = KeyCheckState.Waiting;
                break;
            case KeyCheckState.ServerSuccess:
                //show the message window and close this
                Debug.Log("KeyCheckState: Server Success");
                CloseOutLicenseManager();
                Window_GenericMessage.Inst.Show("License Key Registered",
                    "Thank you for registering.  The software can now be used.", null);
                //todo something weird is going on here, the key isn't validated until a replay, not a reload,
                //and even then the status is invalid and nokeyforgroup..
                    //delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
                break;
            case KeyCheckState.ServerRegFailed:
                Window_GenericMessage.Inst.Show("License Key Error",
                    "Registration was not successful.  Please check the key and try again.  If the problem persists, contact support.");
                m_keyCheckState = KeyCheckState.Waiting;
                break;
            case KeyCheckState.ExistingKeyValidated:
                Debug.Log("KeyCheckState: Existing Key Validated");
                CloseOutLicenseManager();
                break;
            case KeyCheckState.WaitingOnFormFill:
                break;
            case KeyCheckState.WaitingOnServerCheck:
                //accumulate our timing so it doesn't just sit here forever
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CloseOutLicenseManager()
    {
        //once we have a valid key check, show the operator window to require a log on
        if (!m_closedOut)
        {
            m_closedOut = true;
            Manager_Users.Instance.ShowOnStartUp();
        }

        //then just disable this component
        m_keyCheckState = KeyCheckState.Waiting;
        GetComponent<TabThroughFields>().Disable();
        gameObject.SetActive(false);
    }

    private void TextFieldValueChanged(TMP_InputField field)
    {
        //don't alloy any characters beyond 5 to be entered
        var text = field.text;
        if (text.Length > 4) field.text = text.Substring(0, 4);
    }

    public bool GetKeyStatus()
    {
        var status = m_keyManager.CheckLicenseGroups(new[] {"BVTSilver"});

        //check for a valid key, otherwise pop up the window
        Debug.Log("Key Status: " + status);

        return status == BVTLicenseManager.LicenseKeyStatus.OK;
    }

    public override void Show()
    {
        base.Show();

        //clear everything
        m_text_status.text = string.Empty;
        m_text_error.text = string.Empty;
        m_inputFields.ForEach(f => f.text = string.Empty);
    }

    private void ButtonPushed_Back()
    {
        Show();
    }

    private void ButtonPushed_FakeKey()
    {
        m_keyCheckState = KeyCheckState.ExistingKeyValidated;
    }

    private void ButtonPushed_ActivateInternet()
    {
        //don't allow the button to be pushed multiple times...
        if (m_keyCheckState == KeyCheckState.WaitingOnServerCheck) return;

        //get our key string
        var key = CompileKeyString();

        //testing
        //keys
        //todd sample code: SLVR-4ZR4-FDZS-3GBV-2RYN
        //good testing keys:
        //SLVR-3ZY4-FJ6G-6WOF-2UAI
        //SLVR-LSKM-FHGG-6WOF-3NQP
        //SLVR-YKK4-FSOG-6WOF-33QL
        //SLVR-UHD4-FFGG-6WOF-3UAJ
        //SLVR-TT2M-F46G-6WOF-2PYJ
        //key = "";

        var regCheck = VerifyRegistrationFields();
        if (!regCheck)
        {
            m_text_error.text = "Please fill all required Registration fields";
            return;
        }

        var valid = m_keyManager.CheckIfKeyIsValid(key);
        Debug.Log("License Manager - Key: [" + key + "] is Valid? " + valid);
        if (!valid)
        {
            m_text_error.text = "This is not a valid product key";
            return;
        }

        m_keyManager.UpdateKeyringAddLicenseKey(key);

        /*
        BVTLicenseManager.RegistrationInfo regInfo = new BVTLicenseManager.RegistrationInfo
        {
            RegName = "The Name",
            RegCompany = "A Company",
            RegEmail = "Some@one.email",
            RegPhone = "8675309",
            RegAddr1 = "123 elm",
            RegAddr2 = "",
            RegAddr3 = "",
            RegCity = "Nowherevillse",
            RegStateProv = "State",
            RegZipPostalStop = "123456",
            RegCountry = "USA!USA!USA!"
        };
        */

        var regInfo = FillRegistrationInfo();

        // sets the new reg info and writes it to the file. Note like Unity, you can't directly change the fields and expect it to work right.
        m_keyManager.RegistationInfo = regInfo;

        //this should be all we have to do.. then just wait for the event to kick that the validation was successful
        //should get a hit on ServerKeyRegistrationSucceeded if everything is good
        //ServerKeyRegistrationFailed if not...
        //or ServerCommunicationFailure if something went wrong, and should suggest phone
        m_keyManager.StartBackgroundChecking(true);
        m_keyCheckState = KeyCheckState.WaitingOnServerCheck;
        Window_GenericMessage.Inst.Show("Working", "One moment please...");
    }

    private void ButtonPushed_ActivatePhone()
    {
        var key = CompileKeyString();

        var valid = m_keyManager.CheckIfKeyIsValid(key);
        Debug.Log("License Manager - Key: [" + key + "] is Valid? " + valid);
        if (!valid)
        {
            m_text_error.text = "This is not a valid product key";
            return;
        }

        var regCheck = VerifyRegistrationFields();
        if (!regCheck)
        {
            m_text_error.text = "Please fill all required Registration fields";
            return;
        }

        var regInfo = FillRegistrationInfo();
        m_keyManager.RegistationInfo = regInfo;

        Window_PhoneActivation.Instance.Show(m_keyManager.SystemInstallationID, key);
    }

    public bool CheckPhoneActivation(string activationKey)
    {
        var key = CompileKeyString();
        if (m_keyManager.CheckIfKeyAndActivationAreValid(key, activationKey))
        {
            //yay this is valid
            //we'll just manually inject the new state unless this is a bad idea for some reason..
            m_keyManager.UpdateKeyringAddLicenseKey(key);
            m_keyCheckState = KeyCheckState.ServerSuccess;
            return true;
        }
        return false;
    }

    private string CompileKeyString()
    {
        return m_inputFields[0].text + "-" +
               m_inputFields[1].text + "-" +
               m_inputFields[2].text + "-" +
               m_inputFields[3].text + "-" +
               m_inputFields[4].text;
    }

    private bool VerifyRegistrationFields()
    {
        //for now this checks if something is in the field, no actual format checking for email or phone...
        for (var x = 0; x < m_reg_inputFields.Length; x++)
        {
            //index 1 5 and 6 are optional
            if (x == 1 || x == 5 || x == 6) continue;
            if (m_reg_inputFields[x].text.IsNullOrWhitespace()) return false;
        }

        return true;
    }

    private BVTLicenseManager.RegistrationInfo FillRegistrationInfo()
    {
        var regInfo = new BVTLicenseManager.RegistrationInfo
        {
            RegName = m_reg_inputFields[0].text,
            RegCompany = m_reg_inputFields[1].text,
            RegEmail = m_reg_inputFields[2].text,
            RegPhone = m_reg_inputFields[3].text,
            RegAddr1 = m_reg_inputFields[4].text,
            RegAddr2 = m_reg_inputFields[5].text,
            RegAddr3 = m_reg_inputFields[6].text,
            RegCity = m_reg_inputFields[7].text,
            RegStateProv = m_reg_inputFields[8].text,
            RegZipPostalStop = m_reg_inputFields[9].text,
            RegCountry = m_reg_inputFields[10].text
        };

        return regInfo;
    }
    
    

    //License Manager events
    private void ServerCommunicationFailure(BVTLicenseManager.CommFailureResponseCodes responseCode, string msg,
        string key)
    {
        Debug.LogFormat("serverCommunicationFailure {0}, {1}, {2}", responseCode, msg, key);
      if (m_keyCheckState == KeyCheckState.WaitingOnServerCheck) m_keyCheckState = KeyCheckState.ServerCommFailed;
    }

    private void ServerKeyRegistrationFailed(string msg, string key)
    {
        Debug.LogFormat("serverKeyRegistrationFailed {0}, {1}", msg, key);
      if (m_keyCheckState == KeyCheckState.WaitingOnServerCheck) m_keyCheckState = KeyCheckState.ServerRegFailed;
    }

    private void ServerKeyRegistrationSucceeded(string msg, string key)
    {
        Debug.LogFormat("serverKeyRegistrationSucceeded {0}, {1}", msg, key);
        //todo might have to talk to todd if there is a better place to do this...
        m_validatedKey = key;
        if (m_keyCheckState == KeyCheckState.WaitingOnServerCheck) m_keyCheckState = KeyCheckState.ServerSuccess;
        if (m_keyCheckState == KeyCheckState.InitialCheck)
        {
            m_keyCheckState = KeyCheckState.ExistingKeyValidated;
            //m_validatedKey = key;
        }
    }

    private void ServerFinished()
    {
        Debug.Log("Server Finished");
    }

    private void LicenseKeyNowDeactivated(string key)
    {
        Debug.LogFormat("licenseKeyNowDeactivated {0}", key);
    }

    private void LicenseKeyCheckStarted()
    {
        Debug.LogFormat("licenseKeyCheckStarted");
    }

    private void LicenseKeyCheckWorking(int index, int maxCount, string key)
    {
        Debug.LogFormat("licenseKeyCheckWorking {0}, {1}, {2}", index, maxCount, key);
        
   }

   private void LicenseKeyCheckComplete()
    {
        Debug.LogFormat("licenseKeyCheckComplete");
    }
}
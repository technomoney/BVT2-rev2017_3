using System;
using System.Runtime.InteropServices;
using System.Text;

public class BVTLicenseManager : IDisposable
{
   const string dllname = "BertecLicenseManager"; // Unity will pick the proper file in the Plugins/x86 or Plugins/x86_64 folder depending on the platform.
                                                  // the filename must be the same! Notice the lack of .dll which is a new requirement.


   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   public struct LicenseManagerInit
   {
      public string companyName;
      public string applicationName;
      public string applicationVersion;
      public string rootDomain;

      // The key structure info; change these to fit what you need. Currently, you should only use one of two patterns: 20/1/9 (WB,BVT) and 24/2/11 (BA)
      // Passing zero will use the defaults (20/1/9)
      public int keyLength; // length of the license key, minus the dashes
      public int checksumLength;  // length of the trailing checksum char
      public int dataStructLength; // total length of the binary structure

      public bool shouldCacheSystemFingerprint;  // if true, writes to a registry key to avoid Windows 10 upgrade issues (you should do this)
   }

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern IntPtr BLM_Init(ref LicenseManagerInit initData);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern void BLM_Close(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_CheckHandle(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_StartManager(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_StopManager(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_InReadOnlyMode(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_InDemoMode(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_CompletelyUnlicensed(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_DaysUntilKeysExpire(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_KeyringFilename(IntPtr handle, StringBuilder lpString, int nMaxCount);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_SystemFingerprint(IntPtr handle, StringBuilder lpString, int nMaxCount);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_SystemInstallationID(IntPtr handle, StringBuilder lpString, int nMaxCount);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_CondensedKeyString(IntPtr handle, string key, StringBuilder lpString, int nMaxCount);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_FormattedKeyString(IntPtr handle, string key, StringBuilder lpString, int nMaxCount);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_GetKeyStatus(IntPtr handle, string key);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_CheckLicenseGroups(IntPtr handle, string commaGroups);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_IsKeyMarkedAsInvalidated(IntPtr handle, string key);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_CurrentKeyActivationCode(IntPtr handle, string key, StringBuilder lpString, int nMaxCount);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_CheckIfKeyIsValid(IntPtr handle, string key);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_CheckIfKeyAndActivationAreValid(IntPtr handle, string key, string activationCode);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_ValidateCurrentKeyActivation(IntPtr handle, string key);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_UpdateKeyringAddLicenseKeyWithCode(IntPtr handle, string key, string activationCode);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_UpdateKeyringAddLicenseKey(IntPtr handle, string key);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_AddLicenseKeyAndActivationToList(IntPtr handle, string key, string activationCode);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern bool BLM_UpdateKeyringRemoveLicenseKey(IntPtr handle, string key);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_SetBackgroundCheckFrequncy(IntPtr handle, int seconds);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_GetBackgroundCheckFrequncy(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_StartBackgroundChecking(IntPtr handle, bool checkNow);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_StopBackgroundChecking(IntPtr handle);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_GetRegistationInfo(IntPtr handle, IntPtr a);
   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_SetRegistationInfo(IntPtr handle, IntPtr a);


   // Callback stuff

   internal enum BLM_CallbackEventID
   {
      BLMCE_serverCommunicationFailure = 100,
      BLMCE_serverKeyRegistrationFailed,
      BLMCE_serverKeyRegistrationSucceeded,
      BLMCE_serverFinished,
      BLMCE_licenseKeyNowDeactivated,
      BLMCE_licenseKeyCheckStarted,
      BLMCE_licenseKeyCheckWorking,
      BLMCE_licenseKeyCheckComplete,
   };

   [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal delegate void LicenseManagerCallback(IntPtr handle, BLM_CallbackEventID signalType, int num1, int num2,
                                                [MarshalAsAttribute(UnmanagedType.LPStr)] string sMsg,
                                                [MarshalAsAttribute(UnmanagedType.LPStr)] string sKey);

   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_RegisterCallback(IntPtr handle, LicenseManagerCallback cbd);
   [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
   internal static extern int BLM_UnregisterCallback(IntPtr handle, LicenseManagerCallback cbd);
   //////////////////////////////////////////////////////

   internal IntPtr m_handle = IntPtr.Zero;
   internal LicenseManagerCallback m_callback = null;

   internal StringBuilder buffer()
   {
      int length = 512;
      StringBuilder sb = new StringBuilder(length + 1);
      return sb;
   }

   public enum LicenseKeyStatus
   {
      OK = 0,
      ISTRIAL = 1,
      INVALID = -1,
      NOLICENSEFORGROUP = -2,
      KEYNOTINRING = -3,
      EXPIRED = -10,
      REVOKED = -1000
   };

   public enum CommFailureResponseCodes
   {
      NO_ERRORS = 0,
      GENERIC_FAILURE = -1,
      SERVER_SENT_UNKNOWN_RESULT = -2000, // expected a 4 line response starting with v1, didn't get it. Text will have what it was.
      SERVER_SENT_EMPTY_RESULT,   // expected the 4 lines to contain data, at least one was blank.
      SERVER_FAILED_HTTP_CONNECT,   // 404 or something like that.
      UNABLE_TO_DECODE_DATA,      // the resulting data was unable to be decrypted
      UNABLE_TO_ENCODE_DATA,  // this is usually a major failure in the qca module
      DECODED_DATA_CHECKSUM_MISMATCH,   // the decrypted data is apparently bogus
      XML_RESPONSE_MALFORMED,      // the xml parser failed to understand the return data
      UNABLE_TO_WRITE_DATA,      //unable to write to the local file system (fairly catastrophic, probably missing a folder and/or access rights to it)
   };

   public class RegistrationInfo
   {
      public string RegDate = "";
      public string RegName = "";
      public string RegCompany = "";
      public string RegEmail = "";
      public string RegPhone = "";
      public string RegAddr1 = "";
      public string RegAddr2 = "";
      public string RegAddr3 = "";
      public string RegCity = "";
      public string RegStateProv = "";
      public string RegZipPostalStop = "";
      public string RegCountry = "";
   };

   // Internal structure used to convert the registration info from a c-struct to a C# struct
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
   internal struct BLM_RegInfo
   {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegDate;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegCompany;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegEmail;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegPhone;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegAddr1;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegAddr2;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegAddr3;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegCity;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegStateProv;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegZipPostalStop;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string RegCountry;
   }


   // Events


   /// <summary>
   /// Represents the method that handles notifications that the server connection has failed
   /// </summary>
   public delegate void ServerCommunicationFailure(CommFailureResponseCodes responseCode, string msg, string key);
   /// <summary>
   /// Represents the method that handles notifications that the server failed the key (revoked, not valid, etc)
   /// </summary>
   public delegate void ServerKeyRegistrationFailed(string msg, string key);
   /// <summary>
   /// Represents the method that handles notifications that the server passed the key reg
   /// </summary>
   public delegate void ServerKeyRegistrationSucceeded(string msg, string key);
   /// <summary>
   /// Represents the method that handles notifications that the manager is done talking with the server
   /// </summary>
   public delegate void ServerFinished();

   /// <summary>
   /// Represents the method that handles notifications that the key is now dead; you should exit the app or restart at the login screen
   /// </summary>
   public delegate void LicenseKeyNowDeactivated(string key);

   /// <summary>
   /// Represents the method that handles notifications that a license key check has started
   /// </summary>
   public delegate void LicenseKeyCheckStarted();
   /// <summary>
   /// Represents the method that handles notifications that a license key check is in progress
   /// </summary>
   public delegate void LicenseKeyCheckWorking(int index, int maxCount, string key);
   /// <summary>
   /// Represents the method that handles notifications that a license key check has finished
   /// </summary>
   public delegate void LicenseKeyCheckComplete();


   internal ServerCommunicationFailure pOnServerCommunicationFailureEvent;
   internal ServerKeyRegistrationFailed pOnServerKeyRegistrationFailedEvent;
   internal ServerKeyRegistrationSucceeded pOnServerKeyRegistrationSucceededEvent;
   internal ServerFinished pOnServerFinishedEvent;
   internal LicenseKeyNowDeactivated pOnLicenseKeyNowDeactivatedEvent;

   internal LicenseKeyCheckStarted pOnLicenseKeyCheckStartedEvent;
   internal LicenseKeyCheckWorking pOnLicenseKeyCheckWorkingEvent;
   internal LicenseKeyCheckComplete pOnLicenseKeyCheckCompleteEvent;

   // events

   public event ServerCommunicationFailure OnServerCommunicationFailure
   {
      add
      {
         pOnServerCommunicationFailureEvent += value;
      }

      remove
      {
         pOnServerCommunicationFailureEvent -= value;
      }
   }

   public event ServerKeyRegistrationFailed OnServerKeyRegistrationFailed
   {
      add
      {
         pOnServerKeyRegistrationFailedEvent += value;
      }

      remove
      {
         pOnServerKeyRegistrationFailedEvent -= value;
      }
   }

   public event ServerKeyRegistrationSucceeded OnServerKeyRegistrationSucceeded
   {
      add
      {
         pOnServerKeyRegistrationSucceededEvent += value;
      }

      remove
      {
         pOnServerKeyRegistrationSucceededEvent -= value;
      }
   }

   public event ServerFinished OnServerFinished
   {
      add
      {
         pOnServerFinishedEvent += value;
      }

      remove
      {
         pOnServerFinishedEvent -= value;
      }
   }

   public event LicenseKeyNowDeactivated OnLicenseKeyNowDeactivated
   {
      add
      {
         pOnLicenseKeyNowDeactivatedEvent += value;
      }

      remove
      {
         pOnLicenseKeyNowDeactivatedEvent -= value;
      }
   }

   public event LicenseKeyCheckStarted OnLicenseKeyCheckStarted
   {
      add
      {
         pOnLicenseKeyCheckStartedEvent += value;
      }

      remove
      {
         pOnLicenseKeyCheckStartedEvent -= value;
      }
   }

   public event LicenseKeyCheckWorking OnLicenseKeyCheckWorking
   {
      add
      {
         pOnLicenseKeyCheckWorkingEvent += value;
      }

      remove
      {
         pOnLicenseKeyCheckWorkingEvent -= value;
      }
   }

   public event LicenseKeyCheckComplete OnLicenseKeyCheckComplete
   {
      add
      {
         pOnLicenseKeyCheckCompleteEvent += value;
      }

      remove
      {
         pOnLicenseKeyCheckCompleteEvent -= value;
      }
   }





   internal void callbackHandler(IntPtr handle, BLM_CallbackEventID signalType, int num1, int num2,
                                 [MarshalAsAttribute(UnmanagedType.LPStr)] string sMsg,
                                 [MarshalAsAttribute(UnmanagedType.LPStr)] string sKey)
   {
      switch (signalType)
      {
         case BLM_CallbackEventID.BLMCE_serverCommunicationFailure:
            if (pOnServerCommunicationFailureEvent != null)
               pOnServerCommunicationFailureEvent((CommFailureResponseCodes)num1, sMsg, sKey);
            break;
         case BLM_CallbackEventID.BLMCE_serverKeyRegistrationFailed:
            if (pOnServerKeyRegistrationFailedEvent != null)
               pOnServerKeyRegistrationFailedEvent(sMsg, sKey);
            break;
         case BLM_CallbackEventID.BLMCE_serverKeyRegistrationSucceeded:
            if (pOnServerKeyRegistrationSucceededEvent != null)
               pOnServerKeyRegistrationSucceededEvent(sMsg, sKey);
            break;
         case BLM_CallbackEventID.BLMCE_serverFinished:
            if (pOnServerFinishedEvent != null)
               pOnServerFinishedEvent();
            break;
         case BLM_CallbackEventID.BLMCE_licenseKeyNowDeactivated:
            if (pOnLicenseKeyNowDeactivatedEvent != null)
               pOnLicenseKeyNowDeactivatedEvent(sKey);
            break;
         case BLM_CallbackEventID.BLMCE_licenseKeyCheckStarted:
            if (pOnLicenseKeyCheckStartedEvent != null)
               pOnLicenseKeyCheckStartedEvent();
            break;
         case BLM_CallbackEventID.BLMCE_licenseKeyCheckWorking:
            if (pOnLicenseKeyCheckWorkingEvent != null)
               pOnLicenseKeyCheckWorkingEvent(num1, num2, sKey);
            break;
         case BLM_CallbackEventID.BLMCE_licenseKeyCheckComplete:
            if (pOnLicenseKeyCheckCompleteEvent != null)
               pOnLicenseKeyCheckCompleteEvent();
            break;
      }
   }



   //////////////////////////////////////////////////////////////////////////

   public BVTLicenseManager()
   {
      m_callback = callbackHandler;
      LicenseManagerInit initstruct = new LicenseManagerInit();
      initstruct.companyName = "Bertec";
      initstruct.applicationName = "BVT";
      initstruct.applicationVersion = "1.0";
      // leave the root domain alone
      initstruct.shouldCacheSystemFingerprint = true;
      initstruct.keyLength = 20;
      initstruct.checksumLength = 1;
      initstruct.dataStructLength = 9;

      m_handle = BLM_Init(ref initstruct);
      BLM_RegisterCallback(m_handle, m_callback);
   }

   ~BVTLicenseManager()
   {
      Dispose(false);
   }

   public void Dispose()
   {
      Dispose(true);
      GC.SuppressFinalize(this);
   }

   internal void Dispose(bool disposing)
   {
      BLM_UnregisterCallback(m_handle, m_callback);
      BLM_Close(m_handle);
      m_handle = IntPtr.Zero;
      m_callback = null;
   }

   /// <summary>
   /// true if completely unlicensed or the demo has expired
   /// </summary>
   public bool InReadOnlyMode { get { return BLM_InReadOnlyMode(m_handle); } }

   /// <summary>
   /// if all keys are in demo mode, then will return true; if any are legit, will return false.
   /// </summary>
   public bool InDemoMode { get { return BLM_InDemoMode(m_handle); } }

   /// <summary>
   /// if all keys are expired or invalid, then this will be true (implies system reverts to read-only mode)
   /// false means that at least one key is legit, and currentRegInfo should return valid info.
   /// no license keys == unlicensed, returns true
   /// </summary>
   public bool CompletelyUnlicensed { get { return BLM_CompletelyUnlicensed(m_handle); } }

   /// <summary>
   /// if in demo (or expiring mode), then this will return how many days the system has *at least*; least days win if there is multiple demo keys
   /// </summary>
   public int DaysUntilKeysExpire { get { return BLM_DaysUntilKeysExpire(m_handle); } }

   /// <summary>
   /// should be in the program data folder, named <appname>.conf
   /// </summary>
   public string KeyringFilename
   {
      get
      {
         StringBuilder sb = buffer();
         BLM_KeyringFilename(m_handle, sb, sb.Capacity);
         return sb.ToString();
      }
   }


   /// <summary>
   /// returns something like BFQB44WJGM3CQMHYBPMKM3Y2D-00371OEM904465744112-1296805966
   /// </summary>
   public string SystemFingerprint
   {
      get
      {
         StringBuilder sb = buffer();
         BLM_SystemFingerprint(m_handle, sb, sb.Capacity);
         return sb.ToString();
      }
   }

   /// <summary>
   /// returns the installation id like ABC-123456
   /// </summary>
   public string SystemInstallationID
   {
      get
      {
         StringBuilder sb = buffer();
         BLM_SystemInstallationID(m_handle, sb, sb.Capacity);
         return sb.ToString();
      }
   }

   /// <summary>
   /// The current registration information. The reg into should be populated and will be saved in the local keyring;
   /// the information will not be sent to the server until the next time the key is checked or activated. Does not affect
   /// activation or licensing.
   /// </summary>
   public RegistrationInfo RegistationInfo
   {
      get
      {
         BLM_RegInfo blminfo = new BLM_RegInfo();
         IntPtr bptr = Marshal.AllocHGlobal(Marshal.SizeOf(blminfo));
         Marshal.StructureToPtr(blminfo, bptr, false);

         BLM_GetRegistationInfo(m_handle, bptr);

         blminfo = (BLM_RegInfo)Marshal.PtrToStructure(bptr, typeof(BLM_RegInfo));

         Marshal.FreeHGlobal(bptr);

         RegistrationInfo reginfo = new RegistrationInfo
         {
            RegDate = blminfo.RegDate,
            RegName = blminfo.RegName,
            RegCompany = blminfo.RegCompany,
            RegEmail = blminfo.RegEmail,
            RegPhone = blminfo.RegPhone,
            RegAddr1 = blminfo.RegAddr1,
            RegAddr2 = blminfo.RegAddr2,
            RegAddr3 = blminfo.RegAddr3,
            RegCity = blminfo.RegCity,
            RegStateProv = blminfo.RegStateProv,
            RegZipPostalStop = blminfo.RegZipPostalStop,
            RegCountry = blminfo.RegCountry
         };

         return reginfo;
      }
      set
      {
         BLM_RegInfo blminfo = new BLM_RegInfo
         {
            RegDate = value.RegDate,
            RegName = value.RegName,
            RegCompany = value.RegCompany,
            RegEmail = value.RegEmail,
            RegPhone = value.RegPhone,
            RegAddr1 = value.RegAddr1,
            RegAddr2 = value.RegAddr2,
            RegAddr3 = value.RegAddr3,
            RegCity = value.RegCity,
            RegStateProv = value.RegStateProv,
            RegZipPostalStop = value.RegZipPostalStop,
            RegCountry = value.RegCountry
         };

         IntPtr bptr = Marshal.AllocHGlobal(Marshal.SizeOf(blminfo));
         Marshal.StructureToPtr(blminfo, bptr, false);

         BLM_SetRegistationInfo(m_handle, bptr);

         Marshal.FreeHGlobal(bptr);
      }
   }



   /// <summary>
   /// Starts the licnese key management subsystem and loads the keyring; call this after you set the app information (version, name, etc) and before anything else.
   /// </summary>
   public void StartManager()
   {
      BLM_StartManager(m_handle);
   }

   /// <summary>
   /// Shut downs the manager but leaves it still partially active; the keyring is still loaded.
   /// </summary>
   public void StopManager()
   {
      BLM_StopManager(m_handle);
   }

   /// <summary>
   /// returns the key with the spaces and dashes removed.
   /// </summary>
   public string CondensedKeyString(string key)
   {
      StringBuilder sb = buffer();
      BLM_CondensedKeyString(m_handle, key, sb, sb.Capacity);
      return sb.ToString();
   }

   /// <summary>
   /// Returns the key string all prettified with dashes in the correct spots.
   /// </summary>
   public string FormattedKeyString(string key)
   {
      StringBuilder sb = buffer();
      BLM_FormattedKeyString(m_handle, key, sb, sb.Capacity);
      return sb.ToString();
   }

   /// <summary>
   /// Returns a key status enum for the key string
   /// </summary>
   public LicenseKeyStatus KeyStatus(string key)
   {
      int ks = BLM_GetKeyStatus(m_handle, key);
      return (LicenseKeyStatus)ks;
   }

   /// <summary>
   /// Check if have a valid key for a given group
   /// </summary>
   public LicenseKeyStatus CheckLicenseGroups(string[] licenseGroups)
   {
      string csv = string.Join(",", licenseGroups);
      int ks = BLM_CheckLicenseGroups(m_handle, csv);
      return (LicenseKeyStatus)ks;
   }

   /// <summary>
   /// Returns true if the key has been marked
   /// </summary>
   public bool IsKeyMarkedAsInvalidated(string key)
   {
      return BLM_IsKeyMarkedAsInvalidated(m_handle, key);
   }

   /// <summary>
   /// Returns the activation code for the given key from the loaded keyring
   /// </summary>
   public string CurrentKeyActivationCode(string key)
   {
      StringBuilder sb = buffer();
      BLM_CurrentKeyActivationCode(m_handle, key, sb, sb.Capacity);
      return sb.ToString();
   }

   /// <summary>
   /// returns true if the key is legit. The validation is done locally and not over the wire;
   /// this cannot detect key reuse or off-server generated keys. Note that this does NOT activate the key; it just validate the actual key structure itself a
   /// </summary>
   public bool CheckIfKeyIsValid(string key)
   {
      return BLM_CheckIfKeyIsValid(m_handle, key);
   }

   /// <summary>
   /// returns true if the pair is good. The validation is done locally and not over the wire; this cannot detect key reuse.
   /// </summary>
   public bool CheckIfKeyAndActivationAreValid(string key, string activationCode)
   {
      return BLM_CheckIfKeyAndActivationAreValid(m_handle, key, activationCode);
   }

   /// <summary>
   /// returns true if the keyring contains an activation for the key and it's ok and not expired/deactivated.
   /// </summary>
   public bool ValidateCurrentKeyActivation(string key)
   {
      return BLM_ValidateCurrentKeyActivation(m_handle, key);
   }

   /// <summary>
   /// adds a license key to the ring or re-activates it
   /// </summary>
   public bool UpdateKeyringAddLicenseKey(string key, string activationCode)
   {
      return BLM_UpdateKeyringAddLicenseKeyWithCode(m_handle, key, activationCode);
   }

   /// <summary>
   /// same as UpdateKeyringAddLicenseKey but does not contact server nor use the activation value
   /// </summary>
   public bool UpdateKeyringAddLicenseKey(string key)
   {
      return BLM_UpdateKeyringAddLicenseKey(m_handle, key);
   }

   /// <summary>
   /// same as updateKeyringAddLicenseKey but does not contact server nor save the keyring
   /// </summary>
   public int AddLicenseKeyAndActivationToList(string key, string activationCode)
   {
      return BLM_AddLicenseKeyAndActivationToList(m_handle, key, activationCode);
   }

   /// <summary>
   /// removes a key from the keyring
   /// </summary>
   public bool UpdateKeyringRemoveLicenseKey(string key)
   {
      return BLM_UpdateKeyringRemoveLicenseKey(m_handle, key);
   }


   /// <summary>
   /// How frequent the key manager should check the server for license key validation, in seconds. Defaults to 15 days (60*60*24*15).
   /// Set this to 0 to disable automatic background checking; you will need to manually call StartBackgroundChecking(true) yourself instead.
   /// This is only used when StartBackgroundChecking(false) is called, and uses the last-checked registry value to determine if the key
   /// needs re-checked aganst the server.
   /// </summary>
   public int BackgroundCheckFrequncy
   {
      get
      {
         return BLM_GetBackgroundCheckFrequncy(m_handle);
      }
      set
      {
         BLM_SetBackgroundCheckFrequncy(m_handle, value);
      }
   }


   /// <summary>
   /// Starts the background web process that registers the current keys against the server, and updates the reg info if needed.
   /// If checkNow is set, then the process is started immeditaly; otherwise it will be scheduled in the future based on the last
   /// time the license key was checked. Typically this will try to check once every 15 days, but this can be manipulated.
   /// You will want to call this when your app starts up with FALSE, and then again with TRUE whenever you change the reg info
   /// or the license keys. Calling multiple times is ok.
   /// </summary>
   public void StartBackgroundChecking(bool checkNow)
   {
      BLM_StartBackgroundChecking(m_handle, checkNow);
   }

   /// <summary>
   /// Stops the background license key checking. Called automatically during tear down.
   /// </summary>
   public void StopBackgroundChecking()
   {
      BLM_StopBackgroundChecking(m_handle);
   }

}

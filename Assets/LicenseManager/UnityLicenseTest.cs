using UnityEngine;

public class UnityLicenseTest : MonoBehaviour
{
   void serverCommunicationFailure(BVTLicenseManager.CommFailureResponseCodes responseCode, string msg, string key)
   {
      Debug.LogFormat("serverCommunicationFailure {0}, {1}, {2}", responseCode, msg, key);
   }

   void serverKeyRegistrationFailed(string msg, string key)
   {
      Debug.LogFormat("serverKeyRegistrationFailed {0}, {1}", msg, key);
   }

   void serverKeyRegistrationSucceeded(string msg, string key)
   {
      Debug.LogFormat("serverKeyRegistrationSucceeded {0}, {1}", msg, key);
   }

   void serverFinished()
   {
      Debug.LogFormat("serverFinished");
   }

   void licenseKeyNowDeactivated(string key)
   {
      Debug.LogFormat("licenseKeyNowDeactivated {0}", key);
   }

   void licenseKeyCheckStarted()
   {
      Debug.LogFormat("licenseKeyCheckStarted");
   }

   void licenseKeyCheckWorking(int index, int maxCount, string key)
   {
      Debug.LogFormat("licenseKeyCheckWorking {0}, {1}, {2}", index, maxCount, key);
   }

   void licenseKeyCheckComplete()
   {
      Debug.LogFormat("licenseKeyCheckComplete");
   }

   BVTLicenseManager keyMan;


   public bool doStart;
   // Use this for initialization
   void Start()
   {
      keyMan = new BVTLicenseManager();

      // Events will be fired when certain thing happen, mostly when the server is being contacted.
      keyMan.OnServerCommunicationFailure += serverCommunicationFailure;
      keyMan.OnServerKeyRegistrationFailed += serverKeyRegistrationFailed;
      keyMan.OnServerKeyRegistrationSucceeded += serverKeyRegistrationSucceeded;
      keyMan.OnServerFinished += serverFinished;
      keyMan.OnLicenseKeyNowDeactivated += licenseKeyNowDeactivated;

      keyMan.OnLicenseKeyCheckStarted += licenseKeyCheckStarted;
      keyMan.OnLicenseKeyCheckWorking += licenseKeyCheckWorking;
      keyMan.OnLicenseKeyCheckComplete += licenseKeyCheckComplete;

      keyMan.StartManager();

      if (!doStart)
      {
         keyMan.StartBackgroundChecking(false);
         return;
      }
      
      // Pretend we're activating a license key. the TYLR prefix is the Taylor Made Golf private label that we no longer sell,
      // so it's a good choice to test without impacting other stuff.
      string licenseKeyStr = "SLVR-4ZR4-FDZS-3GBV-2RYN";// "TYLR-OAG7-JMHP-G2BF-3OY2";

      bool f = keyMan.CheckIfKeyIsValid(licenseKeyStr);
      Debug.LogFormat("Key {0} valid check result = {1}", licenseKeyStr, f);

      // If the CheckIfKeyIsValid is true, then the key as entered is correct. Note that the key manager strips out spaces and dashes, so those can be left out.
      string compressed = keyMan.CondensedKeyString(licenseKeyStr);
      Debug.LogFormat("CondensedKey: {0} valid check result = {1}", compressed, keyMan.CheckIfKeyIsValid(compressed));

      // case also doesn't matter
      compressed = compressed.ToLower();
      Debug.LogFormat("CondensedLowerCaseKey: {0} valid check result = {1}", compressed, keyMan.CheckIfKeyIsValid(compressed));

      // Pretend the license key was just added, along with registration info.

      // UpdateKeyringAddLicenseKey adds the passed key to the keyring, but does not activate it (the 'a' field is blank) or call the server.
      keyMan.UpdateKeyringAddLicenseKey(licenseKeyStr);

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

      // sets the new reg info and writes it to the file. Note like Unity, you can't directly change the fields and expect it to work right.
      keyMan.RegistationInfo = regInfo;

      BVTLicenseManager.RegistrationInfo regInfo2 = keyMan.RegistationInfo;
      Debug.LogFormat(regInfo2.RegName);
      Debug.LogFormat(regInfo2.RegCompany);
      Debug.LogFormat(regInfo2.RegEmail);
      Debug.LogFormat(regInfo2.RegPhone);
      Debug.LogFormat(regInfo2.RegAddr1);
      Debug.LogFormat(regInfo2.RegCity);
      Debug.LogFormat(regInfo2.RegStateProv);
      Debug.LogFormat(regInfo2.RegZipPostalStop);
      Debug.LogFormat(regInfo2.RegCountry);

      // At this point the local keyring 'conf' file is populated, but not sent to the server yet or activated; the license key is not valid

      // To do this, you can either "activate by phone", which requires the user to call in order email with an Installation ID (system fingerprint)
      // and get a server-generated activation id. For example:
      string installationIDstring = keyMan.SystemInstallationID;  // this should be provided to the user for relay to support
      string activationKey = "ABC-123456";   // this would be provided by tech support and comminicated via phone or email; you would enter into a box. Dashs and spaces will be ignored.
                                             //Y4KJUCJZC is what matches my system
      activationKey = "Y4KJUC---JZC";
      // This will take the license key, the activation key, and the internal SystemInstallationID and check if they combine properly.
      // Simple true/false return; if false, then the pair didn't match and the method will not update the keyring.
      bool passed = keyMan.UpdateKeyringAddLicenseKey(licenseKeyStr, activationKey);
      Debug.LogFormat("UpdateKeyringAddLicenseKey: license {0}, system id {1}, activation {2} result = {3}",
         licenseKeyStr, installationIDstring, activationKey, passed);

      // You can also remove a key, if your user needs to for some reason
      //         keyMan.UpdateKeyringRemoveLicenseKey(licenseKeyStr);


      // To activate over the internet, you will instead want to connect to the proper event signals - 
      // ServerCommunicationFailure, ServerKeyRegistrationFailed, and LicenseKeyCheckComplete usually.
      // Then call StartBackgroundChecking(true) and yield/block until you get a result from the signals. For a simple UI, this is done in a button
      // handler that just starts this and then returns; the ui code would then respond to an event, and if LicenseKeyCheckComplete is sent
      // then the user can be thanked and the "ok" or "close" button programmatically clicked (or dialog otherwise dismissed).

      keyMan.StartBackgroundChecking(true);
   }

   // Update is called once per frame
   void Update()
   {

   }
}

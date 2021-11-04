using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool.Share
{
    public class InstantTwitterShareScript : MonoBehaviour
    {
        public CaptureToolConfig overrideConfig;

        public GameObject twitterPinPanel;
        public InputField tweetInputField;
        public InputField pinInputField;
        public Text pinErrorText;

        public string username;
        public string defaultText;
        public string filePath;

        void OnEnable()
        {
            if (overrideConfig != null && (string.IsNullOrEmpty(username) || username != Twity.Client.screenName))
                overrideConfig.LoadTwitterAuthKeys(username);

            if (string.IsNullOrEmpty(Twity.Oauth.accessToken))
            {
                if (string.IsNullOrEmpty(Twity.Oauth.consumerKey) || string.IsNullOrEmpty(Twity.Oauth.consumerSecret))
                    Debug.LogError("Twitter Error: Twitter consumer key and consumer secret must be set in the config in the share tab of the tool or in an override config on this script.");
                else
                    StartCoroutine(Twity.Client.GenerateRequestToken(RequestTokenCallback));
            }

            tweetInputField.text = defaultText;
        }

        public void SubmitPin()
        {
            pinErrorText.text = "";
            GenerateAccessToken(pinInputField.text);
        }

        public void Share()
        {
            string usernameToUse = username;
            if (string.IsNullOrEmpty(usernameToUse))
                usernameToUse = overrideConfig != null ? overrideConfig.twitterUsername : "";
            APIShare.UploadToTwitter(overrideConfig, filePath, usernameToUse, tweetInputField.text);
            gameObject.SetActive(false);
        }

        void RequestTokenCallback(bool success)
        {
            if (!success)
            {
                Debug.LogError("Request for Twitter token failed");
                return;
            }
            // When request successes, you can display `Twity.Oauth.authorizeURL` to user so that they may use a web browser to access Twitter.
            Application.OpenURL(Twity.Oauth.authorizeURL);
            twitterPinPanel.SetActive(true);
        }

        void GenerateAccessToken(string pin)
        {
            // pin is numbers displayed on web browser when user complete authorization.
            StartCoroutine(Twity.Client.GenerateAccessToken(pin, AccessTokenCallback));
        }

        void AccessTokenCallback(bool success)
        {
            if (!success)
            {
                pinErrorText.text = "Pin Failed";
                return;
            }
            // When success, authorization is completed. You can make request to other endpoint.
            // User's screen_name is in '`Twity.Client.screenName`.

            if (overrideConfig != null)
                overrideConfig.SetTwitterAuthKeys(Twity.Client.screenName, Twity.Oauth.accessToken, Twity.Oauth.accessTokenSecret);
            else
                SavedCaptureToolKeys.SaveTwitterGeneratedAccessToken(Twity.Client.screenName, Twity.Oauth.accessToken, Twity.Oauth.accessTokenSecret);

            twitterPinPanel.SetActive(false);
        }
    }
}
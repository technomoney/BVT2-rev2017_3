using UnityEngine;

namespace TRS.CaptureTool.Share
{
    public class CaptureToolConfig : ScriptableObject
    {
        public bool imgurFreeMode = false;
        public bool imgurAnonymousMode = false;
        public string imgurClientId = ""; // Short alphanumeric value
        public string imgurClientSecret = ""; // Long alphanumeric value
        public string imgurRefreshToken = ""; // Long alphanumeric value
        public string imgurXMashapeKey = ""; // Long alphanumeric value

        public string imgurAccessToken = "";

        public string giphyApiKey = ""; // Long alphanumeric value
        public string giphyUsername = ""; // Required for approved apps only. Otherwise leave empty

        // App information to get approval to tweet on behalf of other users.
        public string twitterConsumerKey = ""; // Short alphanumeric value
        [UnityEngine.Serialization.FormerlySerializedAs("twitterconsumerSecret")]
        public string twitterConsumerSecret = ""; // Long alphanumeric value

        bool useDeveloperTwitter;
        public bool forcePlayerLogIn;
        public string twitterUsername { get { return useDeveloperTwitter ? twitterDeveloperUsername : twitterPlayerUsername; } }
        public string twitterAccessToken { get { return useDeveloperTwitter ? twitterDeveloperAccessToken : twitterPlayerAccessToken; } }
        public string twitterAccessTokenSecret { get { return useDeveloperTwitter ? twitterDeveloperAccessTokenSecret : twitterPlayerAccessTokenSecret; } }

        public string altTwitterUsername { get { return useDeveloperTwitter ? twitterPlayerUsername : twitterDeveloperUsername; } }
        public string altTwitterAccessToken { get { return useDeveloperTwitter ? twitterPlayerAccessToken : twitterDeveloperAccessToken; } }
        public string altTwitterAccessTokenSecret { get { return useDeveloperTwitter ? twitterPlayerAccessTokenSecret : twitterDeveloperAccessTokenSecret; } }


        public string twitterPlayerUsername;
        public string twitterPlayerAccessToken;
        public string twitterPlayerAccessTokenSecret;

        // App information to tweet on the app owner's acccount without need for pin authentication.
        // Either fill out all (username, token, and secret) or none. Code does not check if you filled out each piece.
        public string twitterDeveloperUsername = ""; // Twitter Username - No @
        public string twitterDeveloperAccessToken = ""; // Long alphanumeric value
        public string twitterDeveloperAccessTokenSecret = ""; // Long alphanumeric value

        public string facebookAppId = ""; // 16-digit number

        void Awake()
        {
            LoadKeys();
        }

        public void LoadKeys()
        {
            imgurAccessToken = SavedCaptureToolKeys.ImgurAccessToken();

            useDeveloperTwitter = Application.isEditor && !forcePlayerLogIn;
            if (useDeveloperTwitter && (string.IsNullOrEmpty(twitterDeveloperUsername) || string.IsNullOrEmpty(twitterDeveloperAccessToken) || string.IsNullOrEmpty(twitterDeveloperAccessTokenSecret)))
            {
                twitterDeveloperUsername = SavedCaptureToolKeys.TwitterGeneratedTokenUsername();
                twitterDeveloperAccessToken = SavedCaptureToolKeys.TwitterGeneratedAccessToken();
                twitterDeveloperAccessTokenSecret = SavedCaptureToolKeys.TwitterGeneratedAccessTokenSecret();
            }
            else
            {
                twitterPlayerUsername = SavedCaptureToolKeys.TwitterGeneratedTokenUsername();
                twitterPlayerAccessToken = SavedCaptureToolKeys.TwitterGeneratedAccessToken();
                twitterPlayerAccessTokenSecret = SavedCaptureToolKeys.TwitterGeneratedAccessTokenSecret();
            }

            Twity.Oauth.consumerKey = twitterConsumerKey;
            Twity.Oauth.consumerSecret = twitterConsumerSecret;

            LoadTwitterAuthKeys();
        }

        public bool LoadTwitterAuthKeys(string selectedUsername = "")
        {
            bool hasUsername = selectedUsername.Length != 0;
            if ((hasUsername && twitterUsername == selectedUsername) || (!hasUsername && twitterUsername.Length > 0))
            {
                Twity.Client.screenName = twitterUsername;
                Twity.Oauth.accessToken = twitterAccessToken;
                Twity.Oauth.accessTokenSecret = twitterAccessTokenSecret;
                return true;
            }

            if ((hasUsername && altTwitterUsername == selectedUsername) || (!hasUsername && altTwitterUsername.Length > 0))
            {
                Twity.Client.screenName = altTwitterUsername;
                Twity.Oauth.accessToken = altTwitterAccessToken;
                Twity.Oauth.accessTokenSecret = altTwitterAccessTokenSecret;
                return true;
            }

            return false;
        }

        public void SetTwitterAuthKeys(string username, string accessToken, string accessTokenSecret)
        {
            if (useDeveloperTwitter)
            {
                twitterDeveloperUsername = username;
                twitterDeveloperAccessToken = accessToken;
                twitterDeveloperAccessTokenSecret = accessTokenSecret;
            }
            else
            {
                twitterPlayerUsername = username;
                twitterPlayerAccessToken = accessToken;
                twitterPlayerAccessTokenSecret = accessTokenSecret;
            }

            SavedCaptureToolKeys.SaveTwitterGeneratedAccessToken(Twity.Client.screenName, Twity.Oauth.accessToken, Twity.Oauth.accessTokenSecret);
        }
    }
}
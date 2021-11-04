using UnityEngine;

namespace TRS.CaptureTool.Share
{
    public class InstantScreenshotShareButtonScript : MonoBehaviour
    {
        public ScreenshotScript screenshotScript;
        public ShareScript shareScript;
        public InstantTwitterShareScript instantTwitterShareScript;

        public string username;
        public string extraHashtags;

        bool wasClicked;

        void Awake()
        {
            ScreenshotScript.ScreenshotSaved += ScreenshotSaved;
        }

        void OnDestroy()
        {
            ScreenshotScript.ScreenshotSaved -= ScreenshotSaved;
        }

        public void OnClick()
        {
            wasClicked = true;
            screenshotScript.TakeSingleScreenshot();
        }

        void ScreenshotSaved(string filePath)
        {
            if (!wasClicked)
                return;
            wasClicked = false;

            string hashtagString = "";
            foreach (string hashtag in shareScript.twitterHashtags.Split(','))
                hashtagString += " #" + hashtag;

            foreach (string hashtag in extraHashtags.Split(','))
                hashtagString += " #" + hashtag;

            instantTwitterShareScript.filePath = filePath;
            instantTwitterShareScript.username = username;
            instantTwitterShareScript.defaultText = shareScript.twitterText + hashtagString;
            instantTwitterShareScript.gameObject.SetActive(true);
        }
    }
}
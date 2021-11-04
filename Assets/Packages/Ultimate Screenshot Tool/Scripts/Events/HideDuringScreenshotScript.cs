using UnityEngine;

namespace TRS.CaptureTool
{
    public class HideDuringScreenshotScript : MonoBehaviour
    {
        void Awake()
        {
            ScreenshotScript.WillTakeScreenshot += Disable;
            ScreenshotScript.ScreenshotTaken += Enable;
        }

        void OnDestroy()
        {
            ScreenshotScript.WillTakeScreenshot -= Disable;
            ScreenshotScript.ScreenshotTaken -= Enable;
        }

        void Enable(Texture2D screenshotTexture)
        {
            gameObject.SetActive(true);
        }

        void Disable(int width, int height, int scale)
        {
            gameObject.SetActive(false);
        }
    }
}
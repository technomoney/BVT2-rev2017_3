using UnityEngine;

namespace TRS.CaptureTool
{
    public class ShowDuringScreenshotScript : MonoBehaviour
    {
        void Awake()
        {
            ScreenshotScript.WillTakeScreenshot += Enable;
            ScreenshotScript.ScreenshotTaken += Disable;
        }

        void OnDestroy()
        {
            ScreenshotScript.WillTakeScreenshot -= Enable;
            ScreenshotScript.ScreenshotTaken -= Disable;
        }

        void Enable(int width, int height, int scale)
        {
            gameObject.SetActive(true);
        }

        void Disable(Texture2D screenshotTexture)
        {
            gameObject.SetActive(false);
        }
    }
}
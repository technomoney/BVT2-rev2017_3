using UnityEngine;

namespace TRS.CaptureTool
{
    public class DemoScreenshotCleanUpScript : MonoBehaviour
    {
        public ScreenshotScript screenshotScript;

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
        const string TEMP_DIRECTORY = "Temp";

        bool originalWebPersistValue;
        string originalWebDirectory;

        bool originalMobilePersistValue;
        string originalIOSDirectory;
        string originalAndroidDirectory;

        List<string> createdMediaFiles = new List<string>();

        void OnEnable()
        {
            // There is not direct access to media saved to gallery or to desktop from web, so we need our own persisted file to access (and upload/share) these screenshots
            originalWebPersistValue = screenshotScript.fileSettings.persistLocallyWeb;
            originalWebDirectory = screenshotScript.fileSettings.webDirectory;

            originalMobilePersistValue = screenshotScript.fileSettings.persistLocallyMobile;
            originalAndroidDirectory = screenshotScript.fileSettings.androidDirectory;
            originalIOSDirectory = screenshotScript.fileSettings.iosDirectory;

            screenshotScript.fileSettings.persistLocallyWeb = true;
            screenshotScript.fileSettings.webDirectory = TEMP_DIRECTORY;

            screenshotScript.fileSettings.persistLocallyMobile = true;
            screenshotScript.fileSettings.androidDirectory = TEMP_DIRECTORY;
            screenshotScript.fileSettings.iosDirectory = TEMP_DIRECTORY;

            // Two alternatives demonstrated here. Periodically delete all files in a specific folder or track and delete the persisted files.
            foreach (string filePath in System.IO.Directory.GetFiles(screenshotScript.fileSettings.directory))
                System.IO.File.Delete(filePath);

            ScreenshotScript.ScreenshotSaved += ScreenshotSaved;
        }

        void OnDisable()
        {
            ScreenshotScript.ScreenshotSaved -= ScreenshotSaved;

            foreach (string mediaFilePath in createdMediaFiles)
                System.IO.File.Delete(mediaFilePath);

            screenshotScript.fileSettings.persistLocallyWeb = originalWebPersistValue;
            screenshotScript.fileSettings.webDirectory = originalWebDirectory;

            screenshotScript.fileSettings.persistLocallyMobile = originalMobilePersistValue;
            screenshotScript.fileSettings.androidDirectory = originalAndroidDirectory;
            screenshotScript.fileSettings.iosDirectory = originalIOSDirectory;
        }

        void ScreenshotSaved(string filePath)
        {
            createdMediaFiles.Add(filePath);
        }
#endif
    }
}
﻿namespace TRS.CaptureTool
{
    [System.Serializable]
    public class CaptureFileSettings : FileSettings
    {
        const string CAPTURE_COUNT_KEY = "TRS_CFS_COUNT";

        public string album
        {
            get
            {
#if UNITY_ANDROID
                return androidAlbum;
#elif UNITY_IOS
                return iosAlbum;
#else
                return null;
#endif
            }
        }

        public bool includeCamera;
        public bool includeResolution;

        public bool saveToGallery;
        public string androidAlbum;
        public string iosAlbum;

        public override void SetUp(int uniqueId, string saveType = "Captures")
        {
            base.SetUp(uniqueId, saveType);

            countKey = CAPTURE_COUNT_KEY;

            saveToGallery = true;
            androidAlbum = webDirectory;
            iosAlbum = webDirectory;
        }

        public virtual string FullFilePathWithCaptureDetails(string cameraName, string resolutionName, string resolutionString)
        {
            string folderName = FolderName(ConnectParts(resolutionName, resolutionString));
            string fileName = FileNameWithCaptureDetails(cameraName, resolutionString);
            return FullFilePath(folderName, fileName);
        }

        public string FolderName(params string[] components)
        {
            return FolderNameFromComponents(components);
        }

        public string FileNameWithCaptureDetails(string cameraName, string resolutionString)
        {
            string additionalFileNameComponents = "";

            if (includeCamera && cameraName.Length > 0)
                additionalFileNameComponents += CONNECTING_TEXT + cameraName;

            if (includeResolution && resolutionString.Length > 0)
                additionalFileNameComponents += CONNECTING_TEXT + resolutionString;

            string fileName = FileName(additionalFileNameComponents);
            fileName = fileName.Replace(":", "x").Replace("\"", "in");
            return fileName;
        }

        public string ExampleFileName()
        {
            return FileNameWithCaptureDetails("MainCamera", "1200x800");
        }
    }
}